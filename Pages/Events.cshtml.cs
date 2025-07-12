using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Services;
using JSONAdminEditor.Models;
using System.Text.Json;

namespace JSONAdminEditor.Pages;

public class EventsModel : PageModel
{
    private readonly NotificationsService _notificationsService;

    [BindProperty(SupportsGet = true)]
    public string? SelectedEvent { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsNewEvent { get; set; }

    [BindProperty]
    public string? ActiveTab { get; set; }

    public List<string> ActiveEventTriggers { get; set; } = new();
    public List<string> AvailableOrderTypes { get; set; } = new();
    public List<(string TemplateId, string TemplateName)> AvailableTemplates { get; set; } = new();
    public List<(string TemplateId, string TemplateName)> AvailableEmailTemplates { get; set; } = new();
    public List<(string TemplateId, string TemplateName)> AvailableSmsTemplates { get; set; } = new();
    public List<(string TemplateId, string TemplateName)> AvailableVoiceTemplates { get; set; } = new();
    public Dictionary<string, object>? EventData { get; set; }
    public List<ValidationError> ValidationErrors { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    
    // Content Variables properties
    public Dictionary<string, string>? EventContentVariables { get; set; }
    public Dictionary<string, string>? GlobalContentVariables { get; set; }

    public EventsModel(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    public async Task OnGetAsync()
    {
        await LoadActiveEventTriggersAsync();
        await LoadAvailableOrderTypesAsync();
        await LoadAvailableTemplatesAsync();
        
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
        await LoadAvailableOrderTypesAsync();
        await LoadAvailableTemplatesAsync();
        await LoadEventTemplateAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveEventDataAsync()
    {
        await LoadActiveEventTriggersAsync();
        await LoadAvailableOrderTypesAsync();
        await LoadAvailableTemplatesAsync();

        if (string.IsNullOrEmpty(SelectedEvent))
        {
            ErrorMessage = "No event selected.";
            return Page();
        }

        // Collect form data
        var eventData = new Dictionary<string, object>();
        
        // Get form values
        var orderType = Request.Form["OrderType"].ToString().Trim();
        var phone = Request.Form["Phone"].ToString().Trim();
        var email = Request.Form["Email"].ToString().Trim();
        var logo = Request.Form["Logo"].ToString().Trim();
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
            var availableTypes = string.Join("', '", AvailableOrderTypes);
            ValidationErrors.Add(new ValidationError 
            { 
                FieldName = "OrderType", 
                Message = $"OrderType must be one of: '{availableTypes}'" 
            });
        }

        if (ValidationErrors.Any())
        {
            ErrorMessage = "Please fix the validation errors and try again.";
            // Reload event data to preserve user input
            EventData = new Dictionary<string, object>
            {
                ["OrderType"] = orderType,
                ["Phone"] = phone,
                ["Email"] = email,
                ["Logo"] = logo,
                ["IsSuppressed"] = isSuppressed.ToLower() == "true"
            };
            return Page();
        }

        // Build event data (excluding ContentVariables and Templates for now)
        eventData["OrderType"] = orderType;
        eventData["Phone"] = string.IsNullOrEmpty(phone) ? "" : phone;
        eventData["Email"] = string.IsNullOrEmpty(email) ? "" : email;
        eventData["Logo"] = string.IsNullOrEmpty(logo) ? "" : logo;
        eventData["IsSuppressed"] = isSuppressed.ToLower() == "true";

        // Preserve existing ContentVariables and Templates if they exist (only for existing events)
        if (!IsNewEvent)
        {
            var existingEvent = await _notificationsService.GetEventByNameAsync(SelectedEvent);
            if (existingEvent != null)
            {
                if (existingEvent.TryGetValue("ContentVariables", out var contentVars))
                {
                    eventData["ContentVariables"] = contentVars;
                }
                if (existingEvent.TryGetValue("Templates", out var templates))
                {
                    eventData["Templates"] = templates;
                }
            }
        }
        else
        {
            // For new events, initialize empty ContentVariables and Templates
            eventData["ContentVariables"] = new Dictionary<string, object>();
            eventData["Templates"] = new Dictionary<string, object>
            {
                ["Email"] = "",
                ["Sms"] = "",
                ["Voice"] = ""
            };
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

    public async Task<IActionResult> OnPostSaveTemplatesAsync()
    {
        await LoadActiveEventTriggersAsync();
        await LoadAvailableOrderTypesAsync();
        await LoadAvailableTemplatesAsync();

        if (string.IsNullOrEmpty(SelectedEvent))
        {
            ErrorMessage = "No event selected.";
            return Page();
        }

        // Get the active tab from the form
        ActiveTab = Request.Form["activeTab"].ToString();

        // Get the existing event data
        var existingEvent = await _notificationsService.GetEventByNameAsync(SelectedEvent);
        if (existingEvent == null)
        {
            ErrorMessage = $"Event '{SelectedEvent}' not found.";
            return Page();
        }

        // Get form values for templates
        var emailTemplate = Request.Form["Templates.Email"].ToString().Trim();
        var smsTemplate = Request.Form["Templates.Sms"].ToString().Trim();
        var voiceTemplate = Request.Form["Templates.Voice"].ToString().Trim();

        // Create or update the Templates object
        var templates = new Dictionary<string, object>
        {
            ["Email"] = string.IsNullOrEmpty(emailTemplate) ? "" : emailTemplate,
            ["Sms"] = string.IsNullOrEmpty(smsTemplate) ? "" : smsTemplate,
            ["Voice"] = string.IsNullOrEmpty(voiceTemplate) ? "" : voiceTemplate
        };

        // Update the event data with new templates
        existingEvent["Templates"] = templates;

        // Save the updated event
        var success = await _notificationsService.UpdateEventAsync(SelectedEvent, existingEvent);
        
        if (success)
        {
            SuccessMessage = $"Templates for '{SelectedEvent}' updated successfully!";
            await LoadEventDataAsync(); // Reload to show updated data
        }
        else
        {
            ErrorMessage = "Failed to save templates.";
        }

        return Page();
    }

    private async Task LoadActiveEventTriggersAsync()
    {
        ActiveEventTriggers = await _notificationsService.GetActiveEventTriggersAsync();
    }

    private async Task LoadAvailableOrderTypesAsync()
    {
        AvailableOrderTypes = await _notificationsService.GetAvailableOrderTypesAsync();
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
                ["Phone"] = "$consigneeContact.phone$",
                ["Email"] = "$consigneeContact.email$",
                ["Logo"] = "base64",
                ["IsSuppressed"] = false,
                ["Templates"] = new Dictionary<string, object>
                {
                    ["Email"] = "",
                    ["Sms"] = "",
                    ["Voice"] = ""
                },
                ["ContentVariables"] = new Dictionary<string, object>()
            };
        }
    }

    private async Task LoadAvailableTemplatesAsync()
    {
        var allTemplates = await _notificationsService.GetAvailableTemplatesAsync();
        AvailableTemplates = allTemplates.Select(t => (t.TemplateId, t.TemplateName)).ToList();
        
        // Load channel-specific templates
        AvailableEmailTemplates = await _notificationsService.GetEmailTemplatesAsync();
        AvailableSmsTemplates = await _notificationsService.GetSmsTemplatesAsync();
        AvailableVoiceTemplates = await _notificationsService.GetVoiceTemplatesAsync();
    }

    private bool IsValidOrderType(string orderType)
    {
        return AvailableOrderTypes.Any(ot => ot.Equals(orderType, StringComparison.OrdinalIgnoreCase));
    }
    
    public async Task<IActionResult> OnGetContentVariablesAsync(string eventName)
    {
        try
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return new JsonResult(new { success = false, error = "Event name is required" });
            }
            
            // Load event-specific content variables
            var eventData = await _notificationsService.GetEventByNameAsync(eventName);
            var eventContentVariables = new Dictionary<string, string>();
            
            if (eventData?.TryGetValue("ContentVariables", out var eventVars) == true)
            {
                if (eventVars is Dictionary<string, object> eventVarsDict)
                {
                    // ContentVariables is already an object/dictionary
                    eventContentVariables = eventVarsDict.ToDictionary(
                        kvp => kvp.Key, 
                        kvp => kvp.Value?.ToString() ?? ""
                    );
                }
                else if (eventVars is string eventVarsString && !string.IsNullOrWhiteSpace(eventVarsString))
                {
                    // ContentVariables is a JSON string - parse it
                    try
                    {
                        var parsedVars = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(eventVarsString);
                        if (parsedVars != null)
                        {
                            eventContentVariables = parsedVars.ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value?.ToString() ?? ""
                            );
                        }
                    }
                    catch (JsonException)
                    {
                        // If JSON parsing fails, ignore and continue with empty dictionary
                    }
                }
            }
            
            // Load global content variables
            var globalContentVariables = await _notificationsService.GetGlobalContentVariablesAsync();
            
            return new JsonResult(new
            {
                success = true,
                data = new
                {
                    eventVariables = eventContentVariables,
                    globalVariables = globalContentVariables
                }
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
    
    public async Task<IActionResult> OnPostSaveContentVariablesAsync(string eventName, string contentVariables)
    {
        try
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return new JsonResult(new { success = false, error = "Event name is required" });
            }
            
            // Parse the content variables JSON
            var variablesData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(contentVariables);
            
            if (variablesData == null)
            {
                return new JsonResult(new { success = false, error = "Invalid content variables data" });
            }
            
            // Save the content variables to the event
            var success = await _notificationsService.UpdateEventContentVariablesAsync(eventName, variablesData);
            
            if (success)
            {
                return new JsonResult(new 
                { 
                    success = true, 
                    message = $"Content variables for event '{eventName}' saved successfully!" 
                });
            }
            else
            {
                return new JsonResult(new { success = false, error = "Failed to save content variables" });
            }
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }
}
