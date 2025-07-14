using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JSONAdminEditor.Services;
using JSONAdminEditor.Models;
using System.Text.Json;

namespace JSONAdminEditor.Pages;

public class ContentVariableInfo
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public bool IsRedefined { get; set; } = false;
    public bool IsCustomerSpecific { get; set; } = false;
    public string GlobalValue { get; set; } = "";
}

public class TemplateInfo
{
    public string Channel { get; set; } = "";
    public string Value { get; set; } = "";
    public bool IsRedefined { get; set; } = false;
    public string GlobalValue { get; set; } = "";
    public string DisplayName { get; set; } = "";
}

public class EventDataInfo
{
    public string FieldName { get; set; } = "";
    public string Value { get; set; } = "";
    public bool IsRedefined { get; set; } = false;
    public string GlobalValue { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string FieldType { get; set; } = "text"; // text, checkbox
}

public class CustomerEventsModel : PageModel
{
    private readonly NotificationsService _notificationsService;
    private readonly FileManagementService _fileManagementService;

    [BindProperty(SupportsGet = true)]
    public string? SelectedCustomer { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SelectedEvent { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsNewEvent { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ActiveTab { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SelectedOrderType { get; set; } = "Delivery"; // Default to Delivery

    public bool EventSupportsByOrderType { get; set; } = false;
    public List<string> AvailableEvents { get; set; } = new(); // Events from notifications.json
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
    public Dictionary<string, string>? GlobalEventContentVariables { get; set; } // Content variables from the same event in global settings
    public List<ContentVariableInfo> ContentVariablesList { get; set; } = new();
    
    // Template properties for sophisticated template management
    public Dictionary<string, string>? GlobalEventTemplates { get; set; } // Templates from the same event in global settings
    public List<TemplateInfo> TemplatesList { get; set; } = new();
    
    // Event Data properties for sophisticated event data management
    public Dictionary<string, object>? GlobalEventData { get; set; } // Basic event data from the same event in global settings
    public List<EventDataInfo> EventDataList { get; set; } = new();

    public CustomerEventsModel(NotificationsService notificationsService, FileManagementService fileManagementService)
    {
        _notificationsService = notificationsService;
        _fileManagementService = fileManagementService;
    }

    public async Task OnGetAsync()
    {
        await LoadAvailableEventsAsync(); // Load from notifications.json instead of event-triggers.json
        await LoadAvailableOrderTypesAsync();
        await LoadAvailableTemplatesAsync();
        await LoadGlobalContentVariablesAsync();

        if (!string.IsNullOrEmpty(SelectedCustomer) && !string.IsNullOrEmpty(SelectedEvent))
        {
            // Load event trigger data to check ByOrderType FIRST
            await LoadEventTriggerDataAsync();
            
            // If no specific OrderType is set and this event supports ByOrderType, 
            // try to find the first available variant and set SelectedOrderType accordingly
            if (string.IsNullOrEmpty(SelectedOrderType) && EventSupportsByOrderType)
            {
                var customerEvents = await GetCustomerEventsAsync(SelectedCustomer);
                var eventVariants = customerEvents?.Where(e => 
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
                    // No customer variants exist - default to Delivery
                    SelectedOrderType = "Delivery";
                }
            }
            else if (string.IsNullOrEmpty(SelectedOrderType))
            {
                // For events that don't support ByOrderType, default to Delivery
                SelectedOrderType = "Delivery";
            }
            
            await LoadEventDataAsync();
        }
    }

    private async Task LoadAvailableEventsAsync()
    {
        try
        {
            // Get unique events from notifications.json instead of event-triggers.json
            var globalEvents = await _notificationsService.GetEventsAsync();
            if (globalEvents != null)
            {
                AvailableEvents = globalEvents
                    .Where(e => e.ContainsKey("Event"))
                    .Select(e => e["Event"]?.ToString() ?? "")
                    .Where(eventName => !string.IsNullOrWhiteSpace(eventName))
                    .Distinct()
                    .OrderBy(e => e)
                    .ToList();
            }
        }
        catch (Exception)
        {
            AvailableEvents = new List<string>();
        }
    }

    private async Task LoadAvailableOrderTypesAsync()
    {
        try
        {
            AvailableOrderTypes = await _notificationsService.GetAvailableOrderTypesAsync() ?? new List<string>();
        }
        catch (Exception)
        {
            AvailableOrderTypes = new List<string>();
        }
    }

    private async Task LoadAvailableTemplatesAsync()
    {
        try
        {
            var templates = await _notificationsService.GetAvailableTemplatesAsync();
            if (templates != null)
            {
                AvailableTemplates = templates.Select(t => (t.TemplateId, t.TemplateName)).ToList();

                AvailableEmailTemplates = templates
                    .Where(t => t.ChannelType?.ToLowerInvariant() == "email")
                    .Select(t => (t.TemplateId, t.TemplateName))
                    .ToList();

                AvailableSmsTemplates = templates
                    .Where(t => t.ChannelType?.ToLowerInvariant() == "sms")
                    .Select(t => (t.TemplateId, t.TemplateName))
                    .ToList();

                AvailableVoiceTemplates = templates
                    .Where(t => t.ChannelType?.ToLowerInvariant() == "voice")
                    .Select(t => (t.TemplateId, t.TemplateName))
                    .ToList();
            }
        }
        catch (Exception)
        {
            AvailableTemplates = new List<(string, string)>();
            AvailableEmailTemplates = new List<(string, string)>();
            AvailableSmsTemplates = new List<(string, string)>();
            AvailableVoiceTemplates = new List<(string, string)>();
        }
    }

    private async Task LoadGlobalContentVariablesAsync()
    {
        try
        {
            GlobalContentVariables = await _notificationsService.GetGlobalContentVariablesAsync();
        }
        catch (Exception)
        {
            GlobalContentVariables = new Dictionary<string, string>();
        }
    }

    private async Task LoadEventDataAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedCustomer) || string.IsNullOrEmpty(SelectedEvent))
                return;

            // Load customer-specific event data
            var customerEvents = await GetCustomerEventsAsync(SelectedCustomer);
            if (customerEvents != null)
            {
                EventData = customerEvents.FirstOrDefault(e => 
                    e.TryGetValue("Event", out var eventName) && 
                    eventName?.ToString() == SelectedEvent &&
                    e.TryGetValue("OrderType", out var orderType) &&
                    orderType?.ToString() == SelectedOrderType);
            }

            // Load global event data for the same event
            var globalEventData = await _notificationsService.GetEventByNameAndOrderTypeAsync(SelectedEvent, SelectedOrderType);
            GlobalEventData = globalEventData;
            
            // Load customer-specific content variables
            if (EventData != null && EventData.TryGetValue("ContentVariables", out var cvObj))
            {
                EventContentVariables = ConvertToStringDictionary(cvObj);
            }
            else
            {
                EventContentVariables = new Dictionary<string, string>();
            }

            // Load global event content variables
            if (globalEventData != null && globalEventData.TryGetValue("ContentVariables", out var globalCvObj))
            {
                GlobalEventContentVariables = ConvertToStringDictionary(globalCvObj);
            }
            else
            {
                GlobalEventContentVariables = new Dictionary<string, string>();
            }

            // Load global event templates
            if (globalEventData != null && globalEventData.TryGetValue("Templates", out var globalTemplatesObj))
            {
                GlobalEventTemplates = ConvertToStringDictionary(globalTemplatesObj);
            }
            else
            {
                GlobalEventTemplates = new Dictionary<string, string>();
            }

            // Build the sophisticated content variables list
            BuildContentVariablesList();
            
            // Build the sophisticated templates list
            BuildTemplatesList();
            
            // Build the sophisticated event data list
            BuildEventDataList();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading event data: {ex.Message}";
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

    private void BuildContentVariablesList()
    {
        ContentVariablesList = new List<ContentVariableInfo>();

        // If customer has no content variables for this event, load from global event
        if ((EventContentVariables == null || !EventContentVariables.Any()) && 
            (GlobalEventContentVariables != null && GlobalEventContentVariables.Any()))
        {
            // Load all variables from global event with IsRedefined = false
            foreach (var globalVar in GlobalEventContentVariables)
            {
                ContentVariablesList.Add(new ContentVariableInfo
                {
                    Name = globalVar.Key,
                    Value = globalVar.Value,
                    GlobalValue = globalVar.Value,
                    IsRedefined = false,
                    IsCustomerSpecific = false
                });
            }
        }
        else if (EventContentVariables != null && EventContentVariables.Any())
        {
            // Customer has content variables for this event
            var globalEventVars = GlobalEventContentVariables ?? new Dictionary<string, string>();
            
            // First, add customer-specific variables (not in global)
            var customerSpecificVars = EventContentVariables
                .Where(cv => !globalEventVars.ContainsKey(cv.Key))
                .OrderBy(cv => cv.Key);
                
            foreach (var customerVar in customerSpecificVars)
            {
                ContentVariablesList.Add(new ContentVariableInfo
                {
                    Name = customerVar.Key,
                    Value = customerVar.Value,
                    GlobalValue = "",
                    IsRedefined = false,
                    IsCustomerSpecific = true
                });
            }
            
            // Then, add variables that exist in global (marked as redefined)
            var redefinedVars = EventContentVariables
                .Where(cv => globalEventVars.ContainsKey(cv.Key))
                .OrderBy(cv => cv.Key);
                
            foreach (var redefinedVar in redefinedVars)
            {
                ContentVariablesList.Add(new ContentVariableInfo
                {
                    Name = redefinedVar.Key,
                    Value = redefinedVar.Value,
                    GlobalValue = globalEventVars[redefinedVar.Key],
                    IsRedefined = true,
                    IsCustomerSpecific = false
                });
            }
            
            // Finally, add global variables that are not redefined by customer
            var nonRedefinedGlobalVars = globalEventVars
                .Where(gv => !EventContentVariables.ContainsKey(gv.Key))
                .OrderBy(gv => gv.Key);
                
            foreach (var globalVar in nonRedefinedGlobalVars)
            {
                ContentVariablesList.Add(new ContentVariableInfo
                {
                    Name = globalVar.Key,
                    Value = globalVar.Value,
                    GlobalValue = globalVar.Value,
                    IsRedefined = false,
                    IsCustomerSpecific = false
                });
            }
        }
        
        // Sort: customer-specific first, then others
        ContentVariablesList = ContentVariablesList
            .OrderByDescending(cv => cv.IsCustomerSpecific)
            .ThenBy(cv => cv.Name)
            .ToList();
    }

    private void BuildTemplatesList()
    {
        TemplatesList = new List<TemplateInfo>();
        
        var customerTemplates = GetParsedTemplates();
        var globalTemplates = GlobalEventTemplates ?? new Dictionary<string, string>();
        
        // Define the template channels we support
        var channels = new[] { "Email", "Sms", "Voice" };
        
        foreach (var channel in channels)
        {
            var customerValue = customerTemplates.TryGetValue(channel, out var custVal) ? custVal : "";
            var globalValue = globalTemplates.TryGetValue(channel, out var globalVal) ? globalVal : "";
            
            var templateInfo = new TemplateInfo
            {
                Channel = channel,
                Value = string.IsNullOrEmpty(customerValue) ? globalValue : customerValue,
                GlobalValue = globalValue,
                IsRedefined = !string.IsNullOrEmpty(customerValue),
                DisplayName = channel
            };
            
            TemplatesList.Add(templateInfo);
        }
    }

    private void BuildEventDataList()
    {
        EventDataList = new List<EventDataInfo>();
        
        // Define the event data fields we support
        var eventDataFields = new[]
        {
            new { FieldName = "Phone", DisplayName = "Phone", FieldType = "text" },
            new { FieldName = "Email", DisplayName = "Email", FieldType = "text" },
            new { FieldName = "Logo", DisplayName = "Logo", FieldType = "text" },
            new { FieldName = "IsSuppressed", DisplayName = "Is Suppressed", FieldType = "checkbox" }
        };
        
        foreach (var field in eventDataFields)
        {
            var customerValue = EventData?.TryGetValue(field.FieldName, out var custVal) == true ? custVal?.ToString() ?? "" : "";
            var globalValue = GlobalEventData?.TryGetValue(field.FieldName, out var globalVal) == true ? globalVal?.ToString() ?? "" : "";
            
            var eventDataInfo = new EventDataInfo
            {
                FieldName = field.FieldName,
                DisplayName = field.DisplayName,
                FieldType = field.FieldType,
                Value = string.IsNullOrEmpty(customerValue) ? globalValue : customerValue,
                GlobalValue = globalValue,
                IsRedefined = !string.IsNullOrEmpty(customerValue)
            };
            
            EventDataList.Add(eventDataInfo);
        }
    }

    private async Task<List<Dictionary<string, object>>?> GetCustomerEventsAsync(string customerId)
    {
        try
        {
            var customerData = await _fileManagementService.GetCustomerDataAsync(customerId);
            if (customerData != null && customerData.TryGetValue("Events", out var eventsObj))
            {
                if (eventsObj is List<Dictionary<string, object>> eventsList)
                {
                    return eventsList;
                }
                else if (eventsObj is JsonElement eventsElement && eventsElement.ValueKind == JsonValueKind.Array)
                {
                    var result = new List<Dictionary<string, object>>();
                    foreach (var item in eventsElement.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            dict[prop.Name] = ConvertJsonElement(prop.Value);
                        }
                        result.Add(dict);
                    }
                    return result;
                }
            }
            return new List<Dictionary<string, object>>();
        }
        catch (Exception)
        {
            return new List<Dictionary<string, object>>();
        }
    }

    private object ConvertJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString() ?? "";
            case JsonValueKind.Number:
                return element.TryGetInt32(out var intValue) ? intValue : element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return "";
            case JsonValueKind.Object:
                var dict = new Dictionary<string, object>();
                foreach (var prop in element.EnumerateObject())
                {
                    dict[prop.Name] = ConvertJsonElement(prop.Value);
                }
                return dict;
            case JsonValueKind.Array:
                var list = new List<object>();
                foreach (var item in element.EnumerateArray())
                {
                    list.Add(ConvertJsonElement(item));
                }
                return list;
            default:
                return element.GetRawText();
        }
    }

    private Dictionary<string, string> ConvertToStringDictionary(object? obj)
    {
        var result = new Dictionary<string, string>();
        
        if (obj is Dictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                result[kvp.Key] = kvp.Value?.ToString() ?? "";
            }
        }
        else if (obj is JsonElement element && element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                result[prop.Name] = prop.Value.GetString() ?? "";
            }
        }
        
        return result;
    }

    public async Task<IActionResult> OnGetSearchCustomersAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new JsonResult(new List<CustomerLookupResult>());

        var results = await _fileManagementService.SearchCustomersAsync(term);
        return new JsonResult(results);
    }

    public async Task<IActionResult> OnPostSaveEventAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedCustomer))
            {
                ErrorMessage = "Please select a customer.";
                await OnGetAsync();
                return Page();
            }

            if (string.IsNullOrEmpty(SelectedEvent))
            {
                ErrorMessage = "Please select an event.";
                await OnGetAsync();
                return Page();
            }

            // Get form data - only save redefined event data fields
            var eventData = new Dictionary<string, object>
            {
                ["Event"] = SelectedEvent,
                ["OrderType"] = SelectedOrderType ?? "ALL"
            };

            // Handle Event Data fields - only save redefined fields
            var eventDataFields = new[] { "Phone", "Email", "Logo", "IsSuppressed" };
            
            foreach (var field in eventDataFields)
            {
                var redefinedKey = $"EventDataRedefined.{field}";
                var fieldKey = field;
                
                var isRedefined = Request.Form[redefinedKey].ToString().Contains("true");
                
                if (isRedefined)
                {
                    if (field == "IsSuppressed")
                    {
                        // For checkboxes: if the field exists in form data and contains "true", it's checked
                        // If it doesn't exist or doesn't contain "true", it's unchecked
                        eventData[field] = Request.Form.ContainsKey(fieldKey) && 
                                         Request.Form[fieldKey].ToString().Contains("true");
                    }
                    else
                    {
                        var fieldValue = Request.Form[fieldKey].ToString();
                        // Always save the field value for redefined fields, even if empty
                        eventData[field] = fieldValue;
                    }
                }
            }

            // Handle Templates - only save redefined templates
            var templates = new Dictionary<string, object>();
            var templateChannels = new[] { "Email", "Sms", "Voice" };
            
            foreach (var channel in templateChannels)
            {
                var redefinedKey = $"TemplateRedefined.{channel}";
                var templateKey = $"Templates.{channel}";
                
                var isRedefined = Request.Form[redefinedKey].ToString().Contains("true");
                var templateValue = Request.Form[templateKey].ToString();
                
                // Only include templates that are redefined (customer overrides)
                if (isRedefined && !string.IsNullOrEmpty(templateValue))
                {
                    templates[channel] = templateValue;
                }
            }
            
            eventData["Templates"] = templates;

            // Handle Content Variables - process VarName and VarValue pairs properly
            var contentVariables = new Dictionary<string, string>();
            
            // Get all variable name keys
            var varNameKeys = Request.Form.Keys.Where(k => k.StartsWith("VarName.")).ToList();
            
            foreach (var nameKey in varNameKeys)
            {
                var keyId = nameKey.Substring("VarName.".Length); // Extract the ID part
                var valueKey = $"VarValue.{keyId}";
                var redefinedKey = $"IsRedefined.{keyId}";
                
                var variableName = Request.Form[nameKey].ToString();
                var variableValue = Request.Form[valueKey].ToString();
                var isRedefined = Request.Form[redefinedKey].ToString().Contains("true");
                
                // Only include variables that are redefined (customer overrides) or customer-specific
                if (!string.IsNullOrEmpty(variableName))
                {
                    // For customer-specific variables (those that start with "new_" or don't exist in global)
                    // or for redefined global variables, we include them
                    if (keyId.StartsWith("new_") || isRedefined)
                    {
                        if (!string.IsNullOrEmpty(variableValue))
                        {
                            contentVariables[variableName] = variableValue;
                        }
                    }
                }
            }
            
            eventData["ContentVariables"] = contentVariables;

            // Save to customer file
            await SaveCustomerEventAsync(SelectedCustomer, eventData);

            SuccessMessage = $"Event '{SelectedEvent}' for order type '{SelectedOrderType}' saved successfully for customer '{SelectedCustomer}'.";
            await OnGetAsync();
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving event: {ex.Message}";
            await OnGetAsync();
            return Page();
        }
    }

    private async Task SaveCustomerEventAsync(string customerId, Dictionary<string, object> eventData)
    {
        var customerData = await _fileManagementService.GetCustomerDataAsync(customerId) ?? new Dictionary<string, object>();
        
        // Get existing events or create new list
        var events = new List<Dictionary<string, object>>();
        if (customerData.TryGetValue("Events", out var eventsObj))
        {
            if (eventsObj is List<Dictionary<string, object>> eventsList)
            {
                events = eventsList;
            }
            else if (eventsObj is JsonElement eventsElement && eventsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in eventsElement.EnumerateArray())
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in item.EnumerateObject())
                    {
                        dict[prop.Name] = ConvertJsonElement(prop.Value);
                    }
                    events.Add(dict);
                }
            }
        }

        // Find and update existing event or add new one
        var existingIndex = events.FindIndex(e => 
            e.TryGetValue("Event", out var existingEvent) && 
            existingEvent?.ToString() == eventData["Event"]?.ToString() &&
            e.TryGetValue("OrderType", out var existingOrderType) &&
            existingOrderType?.ToString() == eventData["OrderType"]?.ToString());

        if (existingIndex >= 0)
        {
            events[existingIndex] = eventData;
        }
        else
        {
            events.Add(eventData);
        }

        customerData["Events"] = events;
        await _fileManagementService.SaveCustomerDataAsync(customerId, customerData);
    }

    // Helper method to parse Templates from various formats (JSON string or object)
    public Dictionary<string, string> GetParsedTemplates()
    {
        var result = new Dictionary<string, string>
        {
            ["Email"] = "",
            ["Sms"] = "",
            ["Voice"] = ""
        };

        if (EventData == null || !EventData.TryGetValue("Templates", out var templatesObj))
            return result;

        try
        {
            // Case 1: Templates is already a Dictionary<string, object>
            if (templatesObj is Dictionary<string, object> templatesDict)
            {
                result["Email"] = templatesDict.TryGetValue("Email", out var email) ? email?.ToString() ?? "" : "";
                result["Sms"] = templatesDict.TryGetValue("Sms", out var sms) ? sms?.ToString() ?? "" : "";
                result["Voice"] = templatesDict.TryGetValue("Voice", out var voice) ? voice?.ToString() ?? "" : "";
                return result;
            }

            // Case 2: Templates is a JsonElement (from System.Text.Json)
            if (templatesObj is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.String)
                {
                    // It's a JSON string, parse it
                    var elementJsonString = jsonElement.GetString();
                    if (!string.IsNullOrEmpty(elementJsonString))
                    {
                        var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(elementJsonString);
                        if (parsed != null)
                        {
                            result["Email"] = parsed.TryGetValue("Email", out var email) ? email ?? "" : "";
                            result["Sms"] = parsed.TryGetValue("Sms", out var sms) ? sms ?? "" : "";
                            result["Voice"] = parsed.TryGetValue("Voice", out var voice) ? voice ?? "" : "";
                        }
                    }
                }
                else if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    // It's already a JSON object
                    if (jsonElement.TryGetProperty("Email", out var emailProp))
                        result["Email"] = emailProp.GetString() ?? "";
                    if (jsonElement.TryGetProperty("Sms", out var smsProp))
                        result["Sms"] = smsProp.GetString() ?? "";
                    if (jsonElement.TryGetProperty("Voice", out var voiceProp))
                        result["Voice"] = voiceProp.GetString() ?? "";
                }
                return result;
            }

            // Case 3: Templates is a JSON string
            if (templatesObj is string jsonString && !string.IsNullOrEmpty(jsonString))
            {
                var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
                if (parsed != null)
                {
                    result["Email"] = parsed.TryGetValue("Email", out var email) ? email ?? "" : "";
                    result["Sms"] = parsed.TryGetValue("Sms", out var sms) ? sms ?? "" : "";
                    result["Voice"] = parsed.TryGetValue("Voice", out var voice) ? voice ?? "" : "";
                }
            }
        }
        catch (JsonException)
        {
            // If JSON parsing fails, return empty templates
        }

        return result;
    }
}
