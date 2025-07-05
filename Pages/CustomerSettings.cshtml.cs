using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using System.Text.Json;

namespace JSONAdminEditor.Pages;

public class CustomerSettingsModel : PageModel
{
    private readonly FileManagementService _fileManagementService;

    [BindProperty]
    public CustomerSettingsUploadViewModel Upload { get; set; } = new();

    [BindProperty]
    public CustomerSettingsUploadViewModel PendingUpload { get; set; } = new();

    [TempData]
    public string? TempFilePath { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ConfirmationMessage { get; set; }

    public CustomerSettingsModel(FileManagementService fileManagementService)
    {
        _fileManagementService = fileManagementService;
    }

    public void OnGet()
    {
        CleanupTempFiles();
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        // Get customer name from form data
        if (Request.Form.TryGetValue("Upload.CustomerName", out var customerNameValue))
        {
            Upload.CustomerName = customerNameValue.ToString().Trim();
        }

        // Validate customer name
        if (string.IsNullOrWhiteSpace(Upload.CustomerName))
        {
            ModelState.AddModelError("Upload.CustomerName", "Customer name is required.");
        }

        // Validate customer exists in customers.json
        if (!string.IsNullOrWhiteSpace(Upload.CustomerName))
        {
            var customerExists = await _fileManagementService.ValidateCustomerExistsAsync(Upload.CustomerName);
            if (!customerExists)
            {
                ModelState.AddModelError("Upload.CustomerName", "Customer not found. Please select a valid customer from the suggestions.");
            }
        }

        // File validation
        if (Upload.JsonFile == null || Upload.JsonFile.Length == 0)
        {
            ModelState.AddModelError("Upload.JsonFile", "Please select a JSON file.");
        }

        // Convert to FileUploadViewModel for service call
        var fileUploadModel = new FileUploadViewModel
        {
            FileType = FileType.CustomerSettings,
            CustomerName = Upload.CustomerName,
            CustomerIdForFilename = Upload.CustomerIdForFilename,
            JsonFile = Upload.JsonFile
        };

        var (success, message) = await _fileManagementService.UploadFileAsync(fileUploadModel);

        if (success)
        {
            SuccessMessage = message;
            Upload = new CustomerSettingsUploadViewModel(); // Reset form
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
                PendingUpload = new CustomerSettingsUploadViewModel
                {
                    CustomerName = Upload.CustomerName,
                    CustomerIdForFilename = Upload.CustomerIdForFilename
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
                FileType = FileType.CustomerSettings,
                CustomerName = PendingUpload.CustomerName,
                CustomerIdForFilename = PendingUpload.CustomerIdForFilename,
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
        PendingUpload = new CustomerSettingsUploadViewModel();
        ConfirmationMessage = null;
        TempFilePath = null;

        return Page();
    }

    private string SanitizeCustomerId(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return "";

        // Remove invalid characters and replace with underscores
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(customerId.Where(c => !invalidChars.Contains(c)).ToArray());
        
        // Remove extra spaces and replace with underscores
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"\s+", "_");
        
        return sanitized.ToUpperInvariant();
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

    public async Task<IActionResult> OnGetCustomerOverrideDataAsync(string customerDisplayName)
    {
        if (string.IsNullOrWhiteSpace(customerDisplayName))
            return new JsonResult(new { exists = false });

        try
        {
            // Extract customer ID from display format if needed
            var customerId = customerDisplayName;
            var match = System.Text.RegularExpressions.Regex.Match(customerDisplayName, @"^.+\s\(([^)]+)\)$");
            if (match.Success)
            {
                customerId = match.Groups[1].Value.Trim();
            }

            // Check if customer override exists by looking for the customer file
            var sanitizedId = SanitizeCustomerId(customerId);
            var fileName = $"{sanitizedId}.json";
            var customerFolder = Path.Combine(_fileManagementService.GetDataFolderPath(), "customers");
            var filePath = Path.Combine(customerFolder, fileName);
            
            var exists = System.IO.File.Exists(filePath);
            
            if (exists)
            {
                try
                {
                    var jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent, options);
                    
                    return new JsonResult(new { 
                        exists = true, 
                        data = data,
                        customerDisplayName = customerDisplayName
                    });
                }
                catch
                {
                    return new JsonResult(new { 
                        exists = true, 
                        data = (object?)null,
                        customerDisplayName = customerDisplayName
                    });
                }
            }
            else
            {
                return new JsonResult(new { 
                    exists = false,
                    customerDisplayName = customerDisplayName
                });
            }
        }
        catch (Exception)
        {
            return new JsonResult(new { exists = false });
        }
    }

    public async Task<IActionResult> OnGetCustomerContentVariablesAsync(string customerDisplayName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(customerDisplayName))
            {
                return new JsonResult(new { success = false, error = "Customer name is required" });
            }

            // Extract customer ID from display format
            var customerId = customerDisplayName;
            var match = System.Text.RegularExpressions.Regex.Match(customerDisplayName, @"^.+\s\(([^)]+)\)$");
            if (match.Success)
            {
                customerId = match.Groups[1].Value.Trim();
            }

            // Load customer content variables
            var customerContentVariables = new Dictionary<string, string>();
            var sanitizedId = SanitizeCustomerId(customerId);
            var fileName = $"{sanitizedId}.json";
            var customerFolder = Path.Combine(_fileManagementService.GetDataFolderPath(), "customers");
            var filePath = Path.Combine(customerFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                var jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
                var customerData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                
                if (customerData?.TryGetValue("ContentVariables", out var customerVars) == true)
                {
                    if (customerVars is JsonElement customerVarsElement && customerVarsElement.ValueKind == JsonValueKind.Object)
                    {
                        customerContentVariables = customerVarsElement.EnumerateObject().ToDictionary(
                            prop => prop.Name,
                            prop => prop.Value.GetString() ?? ""
                        );
                    }
                }
            }

            // Load global content variables from notifications.json
            var globalContentVariables = await LoadGlobalContentVariablesAsync();

            return new JsonResult(new
            {
                success = true,
                customerVariables = customerContentVariables,
                globalVariables = globalContentVariables
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }

    public async Task<IActionResult> OnPostSaveCustomerContentVariablesAsync(string customerDisplayName, string contentVariables)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(customerDisplayName))
            {
                return new JsonResult(new { success = false, error = "Customer name is required" });
            }

            // Extract customer ID from display format
            var customerId = customerDisplayName;
            var match = System.Text.RegularExpressions.Regex.Match(customerDisplayName, @"^.+\s\(([^)]+)\)$");
            if (match.Success)
            {
                customerId = match.Groups[1].Value.Trim();
            }

            // Parse the content variables JSON
            var variablesData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(contentVariables);
            
            if (variablesData == null)
            {
                return new JsonResult(new { success = false, error = "Invalid content variables data" });
            }

            // Save to customer file
            var sanitizedId = SanitizeCustomerId(customerId);
            var fileName = $"{sanitizedId}.json";
            var customerFolder = Path.Combine(_fileManagementService.GetDataFolderPath(), "customers");
            var filePath = Path.Combine(customerFolder, fileName);

            // Create customers folder if it doesn't exist
            if (!Directory.Exists(customerFolder))
            {
                Directory.CreateDirectory(customerFolder);
            }

            Dictionary<string, object> customerData;
            
            // Load existing customer data or create new
            if (System.IO.File.Exists(filePath))
            {
                var jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
                customerData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent) ?? new Dictionary<string, object>();
            }
            else
            {
                customerData = new Dictionary<string, object>();
            }

            // Update ContentVariables section
            // Always save if we have variables (even with empty values), only remove if completely empty
            if (variablesData.Count > 0)
            {
                customerData["ContentVariables"] = variablesData;
            }
            else
            {
                customerData.Remove("ContentVariables");
            }

            // Save back to file
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var updatedJson = System.Text.Json.JsonSerializer.Serialize(customerData, options);
            await System.IO.File.WriteAllTextAsync(filePath, updatedJson);

            return new JsonResult(new
            {
                success = true,
                message = $"Content variables for customer '{customerDisplayName}' saved successfully!"
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }

    private async Task<Dictionary<string, string>> LoadGlobalContentVariablesAsync()
    {
        try
        {
            var notificationsPath = Path.Combine(_fileManagementService.GetDataFolderPath(), "notifications.json");
            
            if (!System.IO.File.Exists(notificationsPath))
            {
                return new Dictionary<string, string>();
            }

            var jsonContent = await System.IO.File.ReadAllTextAsync(notificationsPath);
            var document = JsonDocument.Parse(jsonContent);
            
            if (document.RootElement.TryGetProperty("ContentVariables", out var contentVarsElement))
            {
                var result = new Dictionary<string, string>();
                foreach (var prop in contentVarsElement.EnumerateObject())
                {
                    result[prop.Name] = prop.Value.GetString() ?? "";
                }
                return result;
            }
            
            return new Dictionary<string, string>();
        }
        catch (Exception)
        {
            return new Dictionary<string, string>();
        }
    }
}
