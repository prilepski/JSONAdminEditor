using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;

namespace JSONAdminEditor.Pages;

public class DefaultSettingsModel : PageModel
{
    private readonly FileManagementService _fileManagementService;

    [BindProperty]
    public FileUploadViewModel Upload { get; set; } = new();

    [BindProperty]
    public FileUploadViewModel PendingUpload { get; set; } = new();

    [TempData]
    public string? TempFilePath { get; set; }

    public List<ManagedFile> Files { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ConfirmationMessage { get; set; }

    public DefaultSettingsModel(FileManagementService fileManagementService)
    {
        _fileManagementService = fileManagementService;
    }

    public async Task OnGetAsync()
    {
        await LoadFilesAsync();
        CleanupTempFiles();
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        await LoadFilesAsync();

        // Manually parse FileType from form data to avoid enum binding issues
        if (Request.Form.TryGetValue("Upload.FileType", out var fileTypeValue))
        {
            if (int.TryParse(fileTypeValue, out var intValue) && Enum.IsDefined(typeof(FileType), intValue))
            {
                Upload.FileType = (FileType)intValue;
            }
        }

        // Set customer name only for CustomerSettings file type
        if (Upload.FileType == FileType.CustomerSettings)
        {
            if (Request.Form.TryGetValue("Upload.CustomerName", out var customerNameValue))
            {
            Upload.CustomerName = customerNameValue.ToString().Trim();
            }
        }
        else
        {
            Upload.CustomerName = string.Empty;
        }

        // Custom validation for FileType enum (exclude CustomerSettings)
        if (Upload.FileType == FileType.None || Upload.FileType == FileType.CustomerSettings)
        {
            ModelState.AddModelError("Upload.FileType", "Please select a valid file type.");
        }

        // File validation
        if (Upload.JsonFile == null || Upload.JsonFile.Length == 0)
        {
            ModelState.AddModelError("Upload.JsonFile", "Please select a JSON file.");
        }

/*         if (!ModelState.IsValid)
        {
            ErrorMessage = "Please correct the validation errors and try again.";
            return Page();
        }
 */
        var (success, message) = await _fileManagementService.UploadFileAsync(Upload);

        if (success)
        {
            SuccessMessage = message;
            Upload = new FileUploadViewModel(); // Reset form
            await LoadFilesAsync(); // Refresh file list
        }
        else
        {
            // Check if this is a file exists error that needs confirmation
            if (message.Contains("File already exists"))
            {
                // Save the uploaded file temporarily
                var tempDir = Path.Combine(Path.GetTempPath(), "JsonAdminEditor");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);
                
                var tempFileName = Guid.NewGuid().ToString() + ".json";
                TempFilePath = Path.Combine(tempDir, tempFileName);
                
                using (var stream = new FileStream(TempFilePath, FileMode.Create))
                {
                    await Upload.JsonFile!.CopyToAsync(stream);
                }
                
                ConfirmationMessage = message;
                PendingUpload = new FileUploadViewModel
                {
                    FileType = Upload.FileType,
                    CustomerName = Upload.CustomerName
                };
            }
            else
            {
                ErrorMessage = message;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostConfirmOverwriteAsync()
    {
        await LoadFilesAsync();

        if (string.IsNullOrEmpty(TempFilePath) || !System.IO.File.Exists(TempFilePath))
        {
            ErrorMessage = "Temporary file not found. Please try uploading again.";
            return Page();
        }

        try
        {
            // Create a temporary IFormFile from the saved file
            var fileBytes = await System.IO.File.ReadAllBytesAsync(TempFilePath);
            var fileName = "temp.json";
            
            using var stream = new MemoryStream(fileBytes);
            var formFile = new FormFile(stream, 0, fileBytes.Length, "JsonFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/json"
            };

            var uploadModel = new FileUploadViewModel
            {
                FileType = PendingUpload.FileType,
                CustomerName = PendingUpload.CustomerName,
                JsonFile = formFile
            };

            var (success, message) = await _fileManagementService.OverwriteFileAsync(uploadModel);

            if (success)
            {
                SuccessMessage = message;
                await LoadFilesAsync(); // Refresh file list
            }
            else
            {
                ErrorMessage = message;
            }

            // Clean up temp file
            if (System.IO.File.Exists(TempFilePath))
            {
                System.IO.File.Delete(TempFilePath);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error processing confirmation: {ex.Message}";
        }

        // Clear pending upload
        PendingUpload = new FileUploadViewModel();
        ConfirmationMessage = null;
        TempFilePath = null;

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string filePath)
    {
        await LoadFilesAsync();

        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Invalid file path.";
            return Page();
        }

        var success = _fileManagementService.DeleteFile(filePath);

        if (success)
        {
            SuccessMessage = "File deleted successfully.";
            await LoadFilesAsync(); // Refresh file list
        }
        else
        {
            ErrorMessage = "Failed to delete file. File may not exist or cannot be deleted.";
        }

        return Page();
    }

    private async Task LoadFilesAsync()
    {
        // Get all managed files and exclude customer settings
        var allFiles = await _fileManagementService.GetManagedFilesAsync();
        Files = allFiles.Where(f => f.FileType != FileType.CustomerSettings).ToList();
    }

    private void CleanupTempFiles()
    {
        try
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "JsonAdminEditor");
            if (Directory.Exists(tempDir))
            {
                var files = Directory.GetFiles(tempDir, "*.json");
                var cutoff = DateTime.Now.AddHours(-1); // Delete files older than 1 hour
                
                foreach (var file in files)
                {
                    if (System.IO.File.GetCreationTime(file) < cutoff)
                    {
                        try
                        {
                            System.IO.File.Delete(file);
                        }
                        catch
                        {
                            // Ignore errors when deleting temp files
                        }
                    }
                }
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    public async Task<IActionResult> OnGetSearchCustomersAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new JsonResult(new List<CustomerLookupResult>());

        var results = await _fileManagementService.SearchCustomersAsync(term);
        return new JsonResult(results);
    }
}
