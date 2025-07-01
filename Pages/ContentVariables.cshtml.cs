using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Services;
using JSONAdminEditor.Models;
using System.Text.Json;

namespace JSONAdminEditor.Pages;

public class ContentVariablesModel : PageModel
{
    private readonly NotificationsService _notificationsService;

    public List<Dictionary<string, object>> Data { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public ValidationResult ValidationResult { get; set; } = new();

    public ContentVariablesModel(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        try
        {
            var jsonData = Request.Form["jsonData"].ToString();
            
            if (string.IsNullOrEmpty(jsonData))
            {
                ErrorMessage = "No data received.";
                await LoadDataAsync();
                return Page();
            }

            var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonData);
            if (data == null)
            {
                ErrorMessage = "Invalid JSON data format.";
                await LoadDataAsync();
                return Page();
            }

            // Validate uniqueness of Variable Name field
            ValidationResult = ValidateVariableNameUniqueness(data);
            if (!ValidationResult.IsValid)
            {
                Data = data;
                return Page();
            }

            var success = await _notificationsService.UpdateContentVariablesAsync(data);
            
            if (success)
            {
                SuccessMessage = "Content Variables saved successfully!";
                await LoadDataAsync();
            }
            else
            {
                ErrorMessage = "Failed to save changes. Please try again.";
                Data = data;
            }
            
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving data: {ex.Message}";
            await LoadDataAsync();
            return Page();
        }
    }

    private async Task LoadDataAsync()
    {
        var data = await _notificationsService.GetContentVariablesAsync();
        Data = data ?? new List<Dictionary<string, object>>();
    }

    private ValidationResult ValidateVariableNameUniqueness(List<Dictionary<string, object>> data)
    {
        var result = new ValidationResult();
        var seenVariableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var duplicateVariableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // First pass: identify duplicates
        for (int i = 0; i < data.Count; i++)
        {
            var row = data[i];
            if (row.TryGetValue("Variable Name", out var variableNameObj))
            {
                var variableName = variableNameObj?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(variableName))
                {
                    if (!seenVariableNames.Add(variableName))
                    {
                        duplicateVariableNames.Add(variableName);
                    }
                }
                else
                {
                    result.Errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = "Variable Name",
                        Message = "Variable Name cannot be empty"
                    });
                }
            }
            else
            {
                result.Errors.Add(new ValidationError
                {
                    RowIndex = i,
                    FieldName = "Variable Name",
                    Message = "Variable Name is required"
                });
            }
        }

        // Second pass: mark all rows with duplicate values
        for (int i = 0; i < data.Count; i++)
        {
            var row = data[i];
            if (row.TryGetValue("Variable Name", out var variableNameObj))
            {
                var variableName = variableNameObj?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(variableName) && duplicateVariableNames.Contains(variableName))
                {
                    result.Errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = "Variable Name",
                        Message = $"Duplicate Variable Name: '{variableName}' must be unique"
                    });
                }
            }
        }

        result.IsValid = !result.Errors.Any();
        return result;
    }
}
