using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using System.Text.Json;

namespace JSONAdminEditor.Pages;

public class CustomerSettingsModel : PageModel
{
    private readonly FileManagementService _fileManagementService;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public CustomerSettingsModel(FileManagementService fileManagementService)
    {
        _fileManagementService = fileManagementService;
    }

    public void OnGet()
    {
        // Page initialization
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
