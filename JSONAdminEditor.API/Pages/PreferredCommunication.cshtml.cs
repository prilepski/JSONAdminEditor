using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Services;
using JSONAdminEditor.Models;
using System.Text.Json;

namespace JSONAdminEditor.Pages;

public class PreferredCommunicationModel : PageModel
{
    private readonly NotificationsService _notificationsService;
    private readonly UniqueFieldValidationService _validationService;

    public List<Dictionary<string, object>> Data { get; set; } = new();
    public List<string> AvailableChannels { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public ValidationResult ValidationResult { get; set; } = new();

    public PreferredCommunicationModel(NotificationsService notificationsService, UniqueFieldValidationService validationService)
    {
        _notificationsService = notificationsService;
        _validationService = validationService;
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

            // Validate uniqueness of Channel field
            ValidationResult = ValidateChannelUniqueness(data);
            if (!ValidationResult.IsValid)
            {
                Data = data;
                await LoadChannelsAsync();
                return Page();
            }

            // Validate that channels exist in event-channels.json
            var validationErrors = await ValidateChannelsExistAsync(data);
            if (validationErrors.Any())
            {
                ValidationResult.IsValid = false;
                ValidationResult.Errors.AddRange(validationErrors);
                Data = data;
                await LoadChannelsAsync();
                return Page();
            }

            var success = await _notificationsService.UpdatePreferredCommunicationAsync(data);
            
            if (success)
            {
                SuccessMessage = "Preferred Communication settings saved successfully!";
                await LoadDataAsync();
            }
            else
            {
                ErrorMessage = "Failed to save changes. Please try again.";
                Data = data;
                await LoadChannelsAsync();
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
        var data = await _notificationsService.GetPreferredCommunicationAsync();
        Data = data ?? new List<Dictionary<string, object>>();
        await LoadChannelsAsync();
    }

    private async Task LoadChannelsAsync()
    {
        AvailableChannels = await _notificationsService.GetAvailableChannelsAsync();
    }

    private ValidationResult ValidateChannelUniqueness(List<Dictionary<string, object>> data)
    {
        var result = new ValidationResult();
        var seenChannels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var duplicateChannels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // First pass: identify duplicates
        for (int i = 0; i < data.Count; i++)
        {
            var row = data[i];
            if (row.TryGetValue("Channel", out var channelObj))
            {
                var channel = channelObj?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(channel))
                {
                    if (!seenChannels.Add(channel))
                    {
                        duplicateChannels.Add(channel);
                    }
                }
                else
                {
                    result.Errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = "Channel",
                        Message = "Channel cannot be empty"
                    });
                }
            }
            else
            {
                result.Errors.Add(new ValidationError
                {
                    RowIndex = i,
                    FieldName = "Channel",
                    Message = "Channel is required"
                });
            }
        }

        // Second pass: mark all rows with duplicate values
        for (int i = 0; i < data.Count; i++)
        {
            var row = data[i];
            if (row.TryGetValue("Channel", out var channelObj))
            {
                var channel = channelObj?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(channel) && duplicateChannels.Contains(channel))
                {
                    result.Errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = "Channel",
                        Message = $"Duplicate Channel: '{channel}' must be unique"
                    });
                }
            }
        }

        result.IsValid = !result.Errors.Any();
        return result;
    }

    private async Task<List<ValidationError>> ValidateChannelsExistAsync(List<Dictionary<string, object>> data)
    {
        var errors = new List<ValidationError>();
        var availableChannels = await _notificationsService.GetAvailableChannelsAsync();

        for (int i = 0; i < data.Count; i++)
        {
            var row = data[i];
            if (row.TryGetValue("Channel", out var channelObj))
            {
                var channel = channelObj?.ToString()?.Trim()?.ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(channel) && !availableChannels.Contains(channel))
                {
                    errors.Add(new ValidationError
                    {
                        RowIndex = i,
                        FieldName = "Channel",
                        Message = $"Channel '{channel}' does not exist in Event Channels dictionary"
                    });
                }
            }
        }

        return errors;
    }
}
