using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using Newtonsoft.Json;

namespace JSONAdminEditor.Pages;

public class DictionariesModel : PageModel
{
    private readonly FileManagementService _fileManagementService;
    private readonly JsonFileService _jsonFileService;
    private readonly UniqueFieldValidationService _validationService;

    [BindProperty]
    public FileUploadViewModel Upload { get; set; } = new();

    [BindProperty]
    public FileUploadViewModel PendingUpload { get; set; } = new();

    [TempData]
    public string? TempFilePath { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ConfirmationMessage { get; set; }
    
    // JSON Editor properties
    public JsonFileViewModel? JsonData { get; set; }
    public FileType? SelectedFileType { get; set; }

    public DictionariesModel(FileManagementService fileManagementService, JsonFileService jsonFileService, UniqueFieldValidationService validationService)
    {
        _fileManagementService = fileManagementService;
        _jsonFileService = jsonFileService;
        _validationService = validationService;
    }

    public void OnGet()
    {
        CleanupTempFiles();
    }

    public async Task<IActionResult> OnGetLoadDictionaryAsync(int fileType)
    {
        if (Enum.IsDefined(typeof(FileType), fileType))
        {
            SelectedFileType = (FileType)fileType;
            await LoadDictionaryDataAsync(SelectedFileType.Value);
        }
        
        return Page();
    }
    
    public async Task<IActionResult> OnGetDictionaryDataAsync(int fileType)
    {
        try
        {
            if (Enum.IsDefined(typeof(FileType), fileType))
            {
                var selectedFileType = (FileType)fileType;
                var filePath = GetDictionaryFilePath(selectedFileType);
                
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    var jsonData = await _jsonFileService.LoadJsonFileAsync(filePath);
                    
                    if (jsonData.IsValidJson)
                    {
                        return new JsonResult(new
                        {
                            success = true,
                            data = new
                            {
                                columnNames = jsonData.ColumnNames,
                                columnTypes = jsonData.ColumnTypes,
                                tableData = jsonData.TableData,
                                filePath = jsonData.FilePath,
                                fileName = jsonData.FileName,
                                isValidJson = jsonData.IsValidJson
                            }
                        });
                    }
                    else
                    {
                        return new JsonResult(new { success = false, error = jsonData.ErrorMessage });
                    }
                }
                else
                {
                    // Return empty structure for new dictionary
                    return new JsonResult(new
                    {
                        success = true,
                        data = new
                        {
                            columnNames = GetDefaultColumnsForDictionary(selectedFileType),
                            columnTypes = GetDefaultColumnTypesForDictionary(selectedFileType),
                            tableData = new List<Dictionary<string, object>>(),
                            filePath = filePath ?? GetDictionaryFilePath(selectedFileType),
                            fileName = GetDictionaryFileName(selectedFileType),
                            isValidJson = true
                        }
                    });
                }
            }
            
            return new JsonResult(new { success = false, error = "Invalid file type" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
    
    private async Task LoadDictionaryDataAsync(FileType fileType)
    {
        try
        {
            var filePath = GetDictionaryFilePath(fileType);
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                JsonData = await _jsonFileService.LoadJsonFileAsync(filePath);
                
                if (!JsonData.IsValidJson)
                {
                    ErrorMessage = $"Error loading {fileType} dictionary: {JsonData.ErrorMessage}";
                    JsonData = null;
                }
            }
            else
            {
                // Create empty data structure for new dictionary
                JsonData = new JsonFileViewModel
                {
                    FileName = GetDictionaryFileName(fileType),
                    FilePath = filePath ?? GetDictionaryFilePath(fileType),
                    IsValidJson = true,
                    TableData = new List<Dictionary<string, object>>(),
                    ColumnNames = GetDefaultColumnsForDictionary(fileType),
                    ColumnTypes = GetDefaultColumnTypesForDictionary(fileType)
                };
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading dictionary: {ex.Message}";
            JsonData = null;
        }
    }
    
    private string GetDictionaryFilePath(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => Path.Combine("wwwroot", "data", "templates.json"),
            FileType.EventTriggers => Path.Combine("wwwroot", "data", "event-triggers.json"),
            FileType.EventChannels => Path.Combine("wwwroot", "data", "event-channels.json"),
            FileType.OrderTypes => Path.Combine("wwwroot", "data", "order-types.json"),
            FileType.Customers => Path.Combine("wwwroot", "data", "customers.json"),
            _ => string.Empty
        };
    }
    
    private string GetDictionaryFileName(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => "templates.json",
            FileType.EventTriggers => "event-triggers.json",
            FileType.EventChannels => "event-channels.json",
            FileType.OrderTypes => "order-types.json",
            FileType.Customers => "customers.json",
            _ => "unknown.json"
        };
    }
    
    private List<string> GetDefaultColumnsForDictionary(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => new List<string> { "templateId", "templateName", "channelType" },
            FileType.EventTriggers => new List<string> { "Event Name", "IsActive" },
            FileType.EventChannels => new List<string> { "Channel Name", "IsActive" },
            FileType.OrderTypes => new List<string> { "Order Type" },
            FileType.Customers => new List<string> { "customerId", "companyName", "contactPerson", "email", "phone", "address", "IsActive" },
            _ => new List<string> { "id", "name" }
        };
    }
    
    private Dictionary<string, string> GetDefaultColumnTypesForDictionary(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => new Dictionary<string, string> { { "templateId", "text" }, { "templateName", "text" }, { "channelType", "text" } },
            FileType.EventTriggers => new Dictionary<string, string> { { "Event Name", "text" }, { "IsActive", "boolean" } },
            FileType.EventChannels => new Dictionary<string, string> { { "Channel Name", "text" }, { "IsActive", "boolean" } },
            FileType.OrderTypes => new Dictionary<string, string> { { "Order Type", "text" } },
            FileType.Customers => new Dictionary<string, string> { { "customerId", "text" }, { "companyName", "text" }, { "contactPerson", "text" }, { "email", "text" }, { "phone", "text" }, { "address", "text" }, { "IsActive", "boolean" } },
            _ => new Dictionary<string, string> { { "id", "text" }, { "name", "text" } }
        };
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
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

    public async Task<IActionResult> OnPostDirectUploadAsync()
    {
        // Manually parse form data to avoid model binding issues
        var uploadModel = new FileUploadViewModel();

        // Get the file type from form data
        if (Request.Form.TryGetValue("Upload.FileType", out var fileTypeStr))
        {
            if (int.TryParse(fileTypeStr, out var fileTypeInt) && Enum.IsDefined(typeof(FileType), fileTypeInt))
            {
                uploadModel.FileType = (FileType)fileTypeInt;
            }
        }

        // For dictionaries, CustomerName should always be empty
        uploadModel.CustomerName = string.Empty;

        // Get the uploaded file
        if (Request.Form.Files.Count > 0)
        {
            uploadModel.JsonFile = Request.Form.Files[0];
        }

        // Validate required fields
        if (uploadModel.FileType == FileType.None)
        {
            ErrorMessage = "Please select a valid dictionary type.";
            return Page();
        }

        if (uploadModel.JsonFile == null || uploadModel.JsonFile.Length == 0)
        {
            ErrorMessage = "Please select a JSON file to upload.";
            return Page();
        }

        // Validate file extension
        if (!uploadModel.JsonFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "Please select a valid JSON file.";
            return Page();
        }

        // Directly call OverwriteFileAsync since user has already confirmed via overlay
        var (success, message) = await _fileManagementService.OverwriteFileAsync(uploadModel);

        if (success)
        {
            SuccessMessage = message;
            Upload = new FileUploadViewModel(); // Reset form
        }
        else
        {
            ErrorMessage = message;
        }

        return Page();
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

    public async Task<IActionResult> OnPostSaveDictionaryAsync(string filePath, string jsonData, int fileType)
    {
        try
        {
            var tableData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
            if (tableData != null && Enum.IsDefined(typeof(FileType), fileType))
            {
                var selectedFileType = (FileType)fileType;
                
                // Validate uniqueness before saving
                var fileName = GetDictionaryFileName(selectedFileType);
                var validationResult = _validationService.ValidateUniqueness(fileName, tableData);
                
                if (!validationResult.IsValid)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Cannot save: There are validation errors. Please fix duplicate values and try again.",
                        validationErrors = validationResult.Errors.Select(e => new
                        {
                            rowIndex = e.RowIndex,
                            fieldName = e.FieldName,
                            message = e.Message
                        }).ToList()
                    });
                }
                
                var success = await _jsonFileService.SaveJsonFileAsync(filePath, tableData);
                
                if (success)
                {
                    // Return success with updated data
                    var updatedJsonData = await _jsonFileService.LoadJsonFileAsync(filePath);
                    
                    return new JsonResult(new
                    {
                        success = true,
                        message = $"{GetDictionaryDisplayName(selectedFileType)} dictionary saved successfully!",
                        data = new
                        {
                            columnNames = updatedJsonData.ColumnNames,
                            columnTypes = updatedJsonData.ColumnTypes,
                            tableData = updatedJsonData.TableData,
                            filePath = updatedJsonData.FilePath,
                            fileName = updatedJsonData.FileName,
                            isValidJson = updatedJsonData.IsValidJson
                        }
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Failed to save changes."
                    });
                }
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    error = "Invalid data format."
                });
            }
        }
        catch (Exception ex)
        {
            return new JsonResult(new
            {
                success = false,
                error = $"Error saving dictionary: {ex.Message}"
            });
        }
    }
    
    private string GetDictionaryDisplayName(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => "Notification Templates",
            FileType.EventTriggers => "Event Triggers",
            FileType.EventChannels => "Event Channels",
            FileType.OrderTypes => "Order Types",
            FileType.Customers => "Customers",
            _ => "Dictionary"
        };
    }
}
