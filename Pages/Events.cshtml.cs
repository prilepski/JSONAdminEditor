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

    [BindProperty(SupportsGet = true)]
    public string? ActiveTab { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SelectedOrderType { get; set; } = "Delivery"; // Default to Delivery

    public bool EventSupportsByOrderType { get; set; } = false;
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
            // Load event trigger data to check ByOrderType FIRST
            await LoadEventTriggerDataAsync();
            
            // If no specific OrderType is set and this event supports ByOrderType, 
            // try to find the first available variant and set SelectedOrderType accordingly
            if (string.IsNullOrEmpty(SelectedOrderType) && EventSupportsByOrderType)
            {
                var allEventVariants = await _notificationsService.GetEventsAsync();
                var eventVariants = allEventVariants?.Where(e => 
                    e.TryGetValue("Event", out var nameObj) && 
                    nameObj?.ToString()?.Equals(SelectedEvent, StringComparison.OrdinalIgnoreCase) == true).ToList();
                
                if (eventVariants?.Any() == true)
                {
                    // Try to find Delivery first, then Pickup, then any other OrderType
                    var deliveryVariant = eventVariants.FirstOrDefault(e => 
                        e.TryGetValue("OrderType", out var ot) && 
                        ot?.ToString()?.Equals("Delivery", StringComparison.OrdinalIgnoreCase) == true);
                    
                    var pickupVariant = eventVariants.FirstOrDefault(e => 
                        e.TryGetValue("OrderType", out var ot) && 
                        ot?.ToString()?.Equals("Pickup", StringComparison.OrdinalIgnoreCase) == true);
                    
                    if (deliveryVariant != null)
                    {
                        SelectedOrderType = "Delivery";
                    }
                    else if (pickupVariant != null)
                    {
                        SelectedOrderType = "Pickup";
                    }
                    else
                    {
                        // Fall back to the first variant's OrderType
                        var firstVariant = eventVariants.First();
                        if (firstVariant.TryGetValue("OrderType", out var firstOrderType))
                        {
                            SelectedOrderType = firstOrderType?.ToString() ?? "Delivery";
                        }
                        else
                        {
                            SelectedOrderType = "Delivery"; // Default fallback
                        }
                    }
                }
                else
                {
                    // No variants exist - default to Delivery for "add new event" mode
                    SelectedOrderType = "Delivery";
                }
            }
            else if (string.IsNullOrEmpty(SelectedOrderType))
            {
                // For events that don't support ByOrderType, default to Delivery
                SelectedOrderType = "Delivery";
            }
            
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
        await LoadEventTriggerDataAsync(); // Load ByOrderType info
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
            var existingEvent = await _notificationsService.GetEventByNameAndOrderTypeAsync(SelectedEvent, SelectedOrderType);
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
            // Add new event with specific OrderType
            success = await _notificationsService.AddNewEventAsync(SelectedEvent, eventData);
            successMessage = $"New '{orderType}' event node for '{SelectedEvent}' added successfully!";
        }
        else
        {
            // Update existing event using OrderType-aware method - only affects the specific OrderType
            success = await _notificationsService.UpdateEventByOrderTypeAsync(SelectedEvent, SelectedOrderType, eventData);
            successMessage = $"Event '{SelectedEvent}' ({orderType}) updated successfully!";
        }

        if (success)
        {
            SuccessMessage = successMessage;
            IsNewEvent = false; // Reset to normal editing mode
            await LoadEventTriggerDataAsync(); // Reload trigger data to maintain rocker switch
            await LoadEventDataAsync(); // Reload to show updated data
        }
        else
        {
            ErrorMessage = IsNewEvent ? "Failed to add new event." : "Failed to save event data.";
            await LoadEventTriggerDataAsync(); // Still need to reload trigger data for UI consistency
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

        // Get the existing event data using OrderType-aware method
        var existingEvent = await _notificationsService.GetEventByNameAndOrderTypeAsync(SelectedEvent, SelectedOrderType);
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

        // Save the updated event using OrderType-aware method
        var success = await _notificationsService.UpdateEventByOrderTypeAsync(SelectedEvent, SelectedOrderType, existingEvent);
        
        if (success)
        {
            var orderTypePart = EventSupportsByOrderType && !string.IsNullOrEmpty(SelectedOrderType) ? $" ({SelectedOrderType})" : "";
            SuccessMessage = $"Templates for '{SelectedEvent}'{orderTypePart} updated successfully!";
            await LoadEventTriggerDataAsync(); // Reload trigger data to maintain rocker switch
            await LoadEventDataAsync(); // Reload to show updated data
        }
        else
        {
            ErrorMessage = "Failed to save templates.";
            await LoadEventTriggerDataAsync(); // Still need to reload trigger data for UI consistency
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

        // Load event data based on rocker switch state (SelectedOrderType)
        EventData = await _notificationsService.GetEventByNameAndOrderTypeAsync(SelectedEvent, SelectedOrderType);
        
        if (EventData == null)
        {
            // Check if this event supports ByOrderType and if we're looking for a specific OrderType
            if (EventSupportsByOrderType && !string.IsNullOrEmpty(SelectedOrderType))
            {
                // Specific OrderType doesn't exist, prepare for "Add New Event" mode
                IsNewEvent = true;
                await LoadEventTemplateAsync();
                SuccessMessage = $"No '{SelectedOrderType}' variant found for event '{SelectedEvent}'. You can add a new {SelectedOrderType} event node below.";
                ErrorMessage = null; // Clear any error message since this is expected behavior
            }
            else
            {
                ErrorMessage = $"Event '{SelectedEvent}' not found in notifications.json";
            }
        }
        else
        {
            // Event data found, ensure we're not in new event mode
            IsNewEvent = false;
            ErrorMessage = null;
            SuccessMessage = null;
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
            
            // Set the correct OrderType based on rocker switch state
            if (EventSupportsByOrderType && !string.IsNullOrEmpty(SelectedOrderType))
            {
                template["OrderType"] = SelectedOrderType;
            }
            else
            {
                template["OrderType"] = "ALL";
            }
            
            EventData = template;
        }
        else
        {
            // Fallback to default values if template is not found
            var orderType = "ALL";
            if (EventSupportsByOrderType && !string.IsNullOrEmpty(SelectedOrderType))
            {
                orderType = SelectedOrderType;
            }
            
            EventData = new Dictionary<string, object>
            {
                ["Event"] = SelectedEvent,
                ["OrderType"] = orderType,
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
    
    public async Task<IActionResult> OnGetContentVariablesAsync(string eventName, string selectedOrderType)
    {
        try
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return new JsonResult(new { success = false, error = "Event name is required" });
            }
            
            // Load event-specific content variables using OrderType-aware method
            var eventData = await _notificationsService.GetEventByNameAndOrderTypeAsync(eventName, selectedOrderType);
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
    
    public async Task<IActionResult> OnPostSaveContentVariablesAsync(string eventName, string contentVariables, string selectedOrderType)
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
            
            // Save the content variables to the event using OrderType-aware method
            var success = await _notificationsService.UpdateEventContentVariablesByOrderTypeAsync(eventName, selectedOrderType, variablesData);
            
            if (success)
            {
                var orderTypePart = !string.IsNullOrEmpty(selectedOrderType) ? $" ({selectedOrderType})" : "";
                return new JsonResult(new 
                { 
                    success = true, 
                    message = $"Content variables for event '{eventName}'{orderTypePart} saved successfully!" 
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

    public async Task<IActionResult> OnPostSaveAllAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedEvent))
            {
                return new JsonResult(new { success = false, error = "No event selected" });
            }

            // Capture the active tab from the request
            ActiveTab = Request.Form["ActiveTab"].ToString();
            if (string.IsNullOrEmpty(ActiveTab))
            {
                ActiveTab = "event-data"; // Default fallback
            }

            // Step 1: Save Event Data
            await LoadActiveEventTriggersAsync();
            await LoadAvailableOrderTypesAsync();
            await LoadAvailableTemplatesAsync();

            // Collect and validate event data
            var eventData = new Dictionary<string, object>();
            var orderType = Request.Form["OrderType"].ToString().Trim();
            var phone = Request.Form["Phone"].ToString().Trim();
            var email = Request.Form["Email"].ToString().Trim();
            var logo = Request.Form["Logo"].ToString().Trim();
            var isSuppressed = Request.Form["IsSuppressed"].ToString().Trim();

            // Validate required fields
            ValidationErrors.Clear();
            if (string.IsNullOrEmpty(orderType))
            {
                ValidationErrors.Add(new ValidationError 
                { 
                    FieldName = "OrderType", 
                    Message = "OrderType is required" 
                });
            }

            if (ValidationErrors.Any())
            {
                return new JsonResult(new { success = false, error = "Validation errors: " + string.Join(", ", ValidationErrors.Select(e => e.Message)) });
            }

            // Build event data
            eventData["OrderType"] = orderType;
            eventData["Phone"] = string.IsNullOrEmpty(phone) ? "" : phone;
            eventData["Email"] = string.IsNullOrEmpty(email) ? "" : email;
            eventData["Logo"] = string.IsNullOrEmpty(logo) ? "" : logo;
            eventData["IsSuppressed"] = isSuppressed.ToLower() == "true";

            // Step 2: Handle Templates
            var emailTemplate = Request.Form["Templates.Email"].ToString().Trim();
            var smsTemplate = Request.Form["Templates.Sms"].ToString().Trim();
            var voiceTemplate = Request.Form["Templates.Voice"].ToString().Trim();

            var templates = new Dictionary<string, object>
            {
                ["Email"] = string.IsNullOrEmpty(emailTemplate) ? "" : emailTemplate,
                ["Sms"] = string.IsNullOrEmpty(smsTemplate) ? "" : smsTemplate,
                ["Voice"] = string.IsNullOrEmpty(voiceTemplate) ? "" : voiceTemplate
            };
            eventData["Templates"] = templates;

            // Step 3: Handle Content Variables
            var contentVariablesJson = Request.Form["contentVariables"].ToString();
            if (!string.IsNullOrEmpty(contentVariablesJson))
            {
                var contentVariables = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(contentVariablesJson);
                if (contentVariables != null)
                {
                    eventData["ContentVariables"] = contentVariables;
                }
            }
            else
            {
                // Initialize empty ContentVariables if none provided
                eventData["ContentVariables"] = new Dictionary<string, object>();
            }

            // Save all data
            bool success;
            if (IsNewEvent)
            {
                success = await _notificationsService.AddNewEventAsync(SelectedEvent, eventData);
            }
            else
            {
                success = await _notificationsService.UpdateEventByOrderTypeAsync(SelectedEvent, SelectedOrderType, eventData);
            }

            if (success)
            {
                var orderTypePart = !string.IsNullOrEmpty(SelectedOrderType) ? $" ({SelectedOrderType})" : "";
                var action = IsNewEvent ? "added" : "updated";
                return new JsonResult(new 
                { 
                    success = true, 
                    message = $"Event '{SelectedEvent}'{orderTypePart} {action} successfully!",
                    activeTab = ActiveTab
                });
            }
            else
            {
                return new JsonResult(new { success = false, error = "Failed to save event data" });
            }
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, error = ex.Message });
        }
    }

    private async Task LoadEventTriggerDataAsync()
    {
        try
        {
            // Use the NotificationsService method to check if event supports OrderType
            EventSupportsByOrderType = await _notificationsService.CheckEventSupportsByOrderTypeAsync(SelectedEvent ?? "");
        }
        catch (Exception)
        {
            EventSupportsByOrderType = false;
        }
    }

    private bool CheckEventSupportsByOrderType(string? eventName)
    {
        // This method is now deprecated in favor of the async version above
        // Keeping it for compatibility but will be removed in future updates
        if (string.IsNullOrEmpty(eventName))
            return false;

        try
        {
            // This is a synchronous fallback - ideally we should use the async version
            var supportedEvents = new[] { "Schedule Appointment", "Scheduling Reminder", "Final Scheduling Appointment" };
            return supportedEvents.Contains(eventName, StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
