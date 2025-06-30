using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using Newtonsoft.Json;

namespace JSONAdminEditor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly JsonFileService _jsonFileService;
    private readonly UniqueFieldValidationService _validationService;

    public JsonFileViewModel? JsonData { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IndexModel(ILogger<IndexModel> logger, JsonFileService jsonFileService, UniqueFieldValidationService validationService)
    {
        _logger = logger;
        _jsonFileService = jsonFileService;
        _validationService = validationService;
    }

    public async Task OnGet(string? filePath = null)
    {
        // If a file path is provided, load that file
        if (!string.IsNullOrEmpty(filePath))
        {
            await LoadFileFromPath(filePath);
        }
    }

    private async Task LoadFileFromPath(string filePath)
    {
        try
        {
            // Validate that the file exists and is accessible
            if (System.IO.File.Exists(filePath))
            {
                JsonData = await _jsonFileService.LoadJsonFileAsync(filePath);
                
                if (!JsonData.IsValidJson)
                {
                    ErrorMessage = JsonData.ErrorMessage;
                }
                else
                {
                    SuccessMessage = "File loaded successfully!";
                }
            }
            else
            {
                ErrorMessage = "The specified file could not be found.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading file: {ex.Message}";
        }
    }

    public async Task<IActionResult> OnPostSaveAsync(string filePath, string jsonData)
    {
        try
        {
            var tableData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
            if (tableData != null)
            {
                // Validate uniqueness before saving
                var fileName = Path.GetFileName(filePath);
                var validationResult = _validationService.ValidateUniqueness(fileName, tableData);
                
                if (!validationResult.IsValid)
                {
                    ErrorMessage = "Cannot save: There are validation errors. Please fix duplicate values and try again.";
                    // Reload the data with validation errors
                    JsonData = await _jsonFileService.LoadJsonFileAsync(filePath);
                    if (JsonData != null)
                    {
                        JsonData.TableData = tableData; // Use the user's edited data
                        JsonData.ValidationErrors = validationResult.Errors;
                    }
                    return Page();
                }
                
                var success = await _jsonFileService.SaveJsonFileAsync(filePath, tableData);
                
                if (success)
                {
                    SuccessMessage = "Changes saved successfully!";
                    // Reload the data to show updated content
                    JsonData = await _jsonFileService.LoadJsonFileAsync(filePath);
                }
                else
                {
                    ErrorMessage = "Failed to save changes.";
                }
            }
            else
            {
                ErrorMessage = "Invalid data format.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving file: {ex.Message}";
        }

        return Page();
    }
}
