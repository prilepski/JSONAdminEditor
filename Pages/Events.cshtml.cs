using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Services;
using JSONAdminEditor.Models;

namespace JSONAdminEditor.Pages;

public class EventsModel : PageModel
{
    private readonly NotificationsService _notificationsService;

    [BindProperty(SupportsGet = true)]
    public string? SelectedEvent { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsNewEvent { get; set; }

    public List<string> ActiveEventTriggers { get; set; } = new();
    public Dictionary<string, object>? EventData { get; set; }
    public List<ValidationError> ValidationErrors { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public EventsModel(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    public async Task OnGetAsync()
    {
        await LoadActiveEventTriggersAsync();
        
        if (!string.IsNullOrEmpty(SelectedEvent))
        {
            if (IsNewEvent)
            {
                await LoadEventTemplateAsync();
            }
            else
            {
                await LoadEventDataAsync();
            }
        }
    }

    public async Task<IActionResult> OnGetAddEventAsync(string eventName)
    {
        SelectedEvent = eventName;
        IsNewEvent = true;
        await LoadActiveEventTriggersAsync();
        await LoadEventTemplateAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveEventDataAsync()
    {
        await LoadActiveEventTriggersAsync();

        if (string.IsNullOrEmpty(SelectedEvent))
        {
            ErrorMessage = "No event selected.";
            return Page();
        }

        // Collect form data
        var eventData = new Dictionary<string, object>();
        
        // Get form values
        var orderType = Request.Form["OrderType"].ToString().Trim();
        var emailTemplateId = Request.Form["EmailTemplateId"].ToString().Trim();
        var smsTemplateId = Request.Form["SmsTemplateId"].ToString().Trim();
        var voiceTwiMLBaseUrl = Request.Form["VoiceTwiMLBaseUrl"].ToString().Trim();
        var isSuppressed = Request.Form["IsSuppressed"].ToString().Trim();

        // Validate required fields and values
        ValidationErrors.Clear();

        if (string.IsNullOrEmpty(orderType))
        {
            ValidationErrors.Add(new ValidationError 
            { 
                FieldName = "OrderType", 
                Message = "OrderType is required" 
            });
        }
        else if (!IsValidOrderType(orderType))
        {
            ValidationErrors.Add(new ValidationError 
            { 
                FieldName = "OrderType", 
                Message = "OrderType must be 'ALL', 'Pickup', or 'Delivery'" 
            });
        }

        if (ValidationErrors.Any())
        {
            ErrorMessage = "Please fix the validation errors and try again.";
            // Reload event data to preserve user input
            EventData = new Dictionary<string, object>
            {
                ["OrderType"] = orderType,
                ["EmailTemplateId"] = emailTemplateId,
                ["SmsTemplateId"] = smsTemplateId,
                ["VoiceTwiMLBaseUrl"] = voiceTwiMLBaseUrl,
                ["IsSuppressed"] = isSuppressed.ToLower() == "true"
            };
            return Page();
        }

        // Build event data (excluding ContentVariables for now)
        eventData["OrderType"] = orderType;
        eventData["EmailTemplateId"] = string.IsNullOrEmpty(emailTemplateId) ? "" : emailTemplateId;
        eventData["SmsTemplateId"] = string.IsNullOrEmpty(smsTemplateId) ? "" : smsTemplateId;
        eventData["VoiceTwiMLBaseUrl"] = string.IsNullOrEmpty(voiceTwiMLBaseUrl) ? "" : voiceTwiMLBaseUrl;
        eventData["IsSuppressed"] = isSuppressed.ToLower() == "true";

        // Preserve existing ContentVariables if they exist (only for existing events)
        if (!IsNewEvent)
        {
            var existingEvent = await _notificationsService.GetEventByNameAsync(SelectedEvent);
            if (existingEvent != null && existingEvent.TryGetValue("ContentVariables", out var contentVars))
            {
                eventData["ContentVariables"] = contentVars;
            }
        }

        bool success;
        string successMessage;

        if (IsNewEvent)
        {
            // Add new event
            success = await _notificationsService.AddNewEventAsync(SelectedEvent, eventData);
            successMessage = $"Event '{SelectedEvent}' added successfully!";
        }
        else
        {
            // Update existing event
            success = await _notificationsService.UpdateEventAsync(SelectedEvent, eventData);
            successMessage = $"Event '{SelectedEvent}' updated successfully!";
        }

        if (success)
        {
            SuccessMessage = successMessage;
            IsNewEvent = false; // Reset to normal editing mode
            await LoadEventDataAsync(); // Reload to show updated data
        }
        else
        {
            ErrorMessage = IsNewEvent ? "Failed to add new event." : "Failed to save event data.";
        }

        return Page();
    }

    private async Task LoadActiveEventTriggersAsync()
    {
        ActiveEventTriggers = await _notificationsService.GetActiveEventTriggersAsync();
    }

    private async Task LoadEventDataAsync()
    {
        if (string.IsNullOrEmpty(SelectedEvent))
            return;

        EventData = await _notificationsService.GetEventByNameAsync(SelectedEvent);
        
        if (EventData == null)
        {
            ErrorMessage = $"Event '{SelectedEvent}' not found in notifications.json";
        }
    }

    private async Task LoadEventTemplateAsync()
    {
        if (string.IsNullOrEmpty(SelectedEvent))
            return;

        var template = await _notificationsService.GetEventTemplateAsync();
        
        if (template != null)
        {
            // Set the Event field to the selected event name
            template["Event"] = SelectedEvent;
            EventData = template;
        }
        else
        {
            // Fallback to default values if template is not found
            EventData = new Dictionary<string, object>
            {
                ["Event"] = SelectedEvent,
                ["OrderType"] = "ALL",
                ["EmailTemplateId"] = "",
                ["SmsTemplateId"] = "",
                ["VoiceTwiMLBaseUrl"] = "",
                ["IsSuppressed"] = false
            };
        }
    }

    private static bool IsValidOrderType(string orderType)
    {
        return orderType.Equals("ALL", StringComparison.OrdinalIgnoreCase) ||
               orderType.Equals("Pickup", StringComparison.OrdinalIgnoreCase) ||
               orderType.Equals("Delivery", StringComparison.OrdinalIgnoreCase);
    }
}
