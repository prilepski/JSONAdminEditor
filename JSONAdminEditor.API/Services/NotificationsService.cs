using JSONAdminEditor.Models;
using JSONAdminEditor.Application.Models;
using System.Text.Json;

namespace JSONAdminEditor.Services
{
    public class NotificationsService
    {
        private readonly IFileContentService _fileContentService;
        private readonly string _notificationsFilePath;

        public NotificationsService(IFileContentService fileContentService)
        {
            _fileContentService = fileContentService;
            _notificationsFilePath = "data/notifications.json";
        }

        public async Task<List<Dictionary<string, object>>?> GetPreferredCommunicationAsync()
        {
            try
            {
                if (!(await _fileContentService.FileExistsAsync(_notificationsFilePath)))
                {
                    return new List<Dictionary<string, object>>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                var document = JsonDocument.Parse(jsonContent);
                
                if (document.RootElement.TryGetProperty("PreferredCommunication", out var prefCommElement))
                {
                    var result = new List<Dictionary<string, object>>();
                    
                    foreach (var item in prefCommElement.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            dict[prop.Name] = ParseJsonPropertyValue(prop);
                        }
                        result.Add(dict);
                    }
                    
                    return result;
                }
                
                return new List<Dictionary<string, object>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdatePreferredCommunicationAsync(List<Dictionary<string, object>> preferredCommunication)
        {
            try
            {
                // Read the entire notifications.json file
                var existingContent = new Dictionary<string, object>();
                
                if (await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                    var document = JsonDocument.Parse(jsonContent);
                    
                    // Preserve existing sections
                    foreach (var prop in document.RootElement.EnumerateObject())
                    {
                        if (prop.Name != "PreferredCommunication")
                        {
                            var deserializedValue = JsonSerializer.Deserialize<object>(prop.Value.GetRawText());
                            if (deserializedValue != null)
                            {
                                existingContent[prop.Name] = deserializedValue;
                            }
                        }
                    }
                }
                
                // Update the PreferredCommunication section
                existingContent["PreferredCommunication"] = preferredCommunication;
                
                // Write back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                var updatedJson = JsonSerializer.Serialize(existingContent, options);
                await _fileContentService.WriteFileAsync(_notificationsFilePath, updatedJson);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<string>> GetAvailableChannelsAsync()
        {
            try
            {
                var channelsFilePath = "data/dictionaries/event-channels.json";
                
                if (!await _fileContentService.FileExistsAsync(channelsFilePath))
                {
                    return new List<string>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(channelsFilePath);
                var channels = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);
                
                if (channels == null) return new List<string>();
                
                return channels
                    .Where(c => c.ContainsKey("Channel Name"))
                    .Select(c => c["Channel Name"]?.ToString()?.ToLowerInvariant() ?? "")
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<Dictionary<string, object>>?> GetContentVariablesAsync()
        {
            try
            {
                if (!await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    return new List<Dictionary<string, object>>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                var document = JsonDocument.Parse(jsonContent);
                
                if (document.RootElement.TryGetProperty("ContentVariables", out var contentVarsElement))
                {
                    var result = new List<Dictionary<string, object>>();
                    
                    foreach (var prop in contentVarsElement.EnumerateObject())
                    {
                        var dict = new Dictionary<string, object>
                        {
                            ["Variable Name"] = prop.Name,
                            ["Variable Mapping"] = ParseJsonPropertyValue(prop)
                        };
                        result.Add(dict);
                    }
                    
                    return result;
                }
                
                return new List<Dictionary<string, object>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateContentVariablesAsync(List<Dictionary<string, object>> contentVariables)
        {
            try
            {
                // Read the entire notifications.json file
                var existingContent = new Dictionary<string, object>();
                
                if (await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                    var document = JsonDocument.Parse(jsonContent);
                    
                    // Preserve existing sections
                    foreach (var prop in document.RootElement.EnumerateObject())
                    {
                        if (prop.Name != "ContentVariables")
                        {
                            var deserializedValue = JsonSerializer.Deserialize<object>(prop.Value.GetRawText());
                            if (deserializedValue != null)
                            {
                                existingContent[prop.Name] = deserializedValue;
                            }
                        }
                    }
                }
                
                // Convert the list back to dictionary format for ContentVariables
                var contentVarsDict = new Dictionary<string, object>();
                foreach (var item in contentVariables)
                {
                    if (item.TryGetValue("Variable Name", out var nameObj) && 
                        item.TryGetValue("Variable Mapping", out var mappingObj))
                    {
                        var name = nameObj?.ToString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            contentVarsDict[name] = mappingObj?.ToString() ?? "";
                        }
                    }
                }
                
                // Update the ContentVariables section
                existingContent["ContentVariables"] = contentVarsDict;
                
                // Write back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                var updatedJson = JsonSerializer.Serialize(existingContent, options);
                await _fileContentService.WriteFileAsync(_notificationsFilePath, updatedJson);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Dictionary<string, object>>?> GetEventsAsync()
        {
            try
            {
                if (!await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    return new List<Dictionary<string, object>>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                var document = JsonDocument.Parse(jsonContent);
                
                if (document.RootElement.TryGetProperty("Events", out var eventsElement))
                {
                    var result = new List<Dictionary<string, object>>();
                    
                    foreach (var item in eventsElement.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            dict[prop.Name] = ParseJsonPropertyValue(prop);
                        }
                        result.Add(dict);
                    }
                    
                    return result;
                }
                
                return new List<Dictionary<string, object>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Dictionary<string, object>?> GetEventByNameAsync(string eventName)
        {
            var events = await GetEventsAsync();
            if (events == null) return null;

            return events.FirstOrDefault(e => 
                e.TryGetValue("Event", out var nameObj) && 
                nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true);
        }

        public async Task<Dictionary<string, object>?> GetEventByNameAndOrderTypeAsync(string eventName, string? orderType = null)
        {
            var events = await GetEventsAsync();
            if (events == null) return null;

            // Check if this event supports ByOrderType
            var eventSupportsOrderType = await CheckEventSupportsByOrderTypeAsync(eventName);
            
            // Filter events by name first
            var matchingEvents = events.Where(e => 
                e.TryGetValue("Event", out var nameObj) && 
                nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true).ToList();

            if (!matchingEvents.Any()) return null;

            // If event doesn't support order type, return the first match (usually "ALL")
            if (!eventSupportsOrderType)
            {
                return matchingEvents.FirstOrDefault(e => 
                    e.TryGetValue("OrderType", out var orderTypeObj) && 
                    orderTypeObj?.ToString()?.Equals("ALL", StringComparison.OrdinalIgnoreCase) == true) 
                    ?? matchingEvents.FirstOrDefault();
            }

            // If no specific order type requested, return "ALL" version
            if (string.IsNullOrEmpty(orderType))
            {
                return matchingEvents.FirstOrDefault(e => 
                    e.TryGetValue("OrderType", out var orderTypeObj) && 
                    orderTypeObj?.ToString()?.Equals("ALL", StringComparison.OrdinalIgnoreCase) == true) 
                    ?? matchingEvents.FirstOrDefault();
            }

            // Look for specific OrderType match - if not found, return null (so "add new event" can be shown)
            var specificOrderTypeEvent = matchingEvents.FirstOrDefault(e => 
                e.TryGetValue("OrderType", out var orderTypeObj) && 
                orderTypeObj?.ToString()?.Equals(orderType, StringComparison.OrdinalIgnoreCase) == true);

            return specificOrderTypeEvent; // Return null if not found
        }

        public async Task<bool> CheckEventSupportsByOrderTypeAsync(string eventName)
        {
            try
            {
                var eventTriggersFilePath = "data/dictionaries/event-triggers.json";
                
                if (!await _fileContentService.FileExistsAsync(eventTriggersFilePath))
                {
                    return false;
                }

                var jsonContent = await _fileContentService.ReadFileAsync(eventTriggersFilePath);
                var triggers = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);
                
                if (triggers == null) return false;
                
                var eventTrigger = triggers.FirstOrDefault(t => 
                    t.TryGetValue("Event Name", out var nameObj) && 
                    nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true);

                if (eventTrigger != null && eventTrigger.TryGetValue("ByOrderType", out var byOrderTypeObj))
                {
                    if (byOrderTypeObj is bool byOrderTypeBool)
                        return byOrderTypeBool;
                    
                    if (bool.TryParse(byOrderTypeObj?.ToString(), out var byOrderTypeParsed))
                        return byOrderTypeParsed;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEventAsync(string eventName, Dictionary<string, object> eventData)
        {
            // Use the same logic as AddNewEventAsync - remove existing and add new
            return await AddNewEventAsync(eventName, eventData);
        }

        public async Task<bool> UpdateEventByOrderTypeAsync(string eventName, string? orderType, Dictionary<string, object> eventData)
        {
            try
            {
                // Read the entire notifications.json file
                var existingContent = new Dictionary<string, object>();
                
                if (await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                    var document = JsonDocument.Parse(jsonContent);
                    
                    // Preserve existing sections
                    foreach (var prop in document.RootElement.EnumerateObject())
                    {
                        var deserializedValue = JsonSerializer.Deserialize<object>(prop.Value.GetRawText());
                        if (deserializedValue != null)
                        {
                            existingContent[prop.Name] = deserializedValue;
                        }
                    }
                }

                // Check if this event supports ByOrderType
                var eventSupportsOrderType = await CheckEventSupportsByOrderTypeAsync(eventName);
                
                // Get existing Events array
                var eventsList = new List<Dictionary<string, object>>();
                if (existingContent.TryGetValue("Events", out var eventsObj) && eventsObj is JsonElement eventsElement)
                {
                    foreach (var item in eventsElement.EnumerateArray())
                    {
                        var eventDict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            eventDict[prop.Name] = ParseJsonPropertyValue(prop);
                        }
                        eventsList.Add(eventDict);
                    }
                }

                // Find the event to update
                Dictionary<string, object>? eventToUpdate = null;
                
                if (!eventSupportsOrderType || string.IsNullOrEmpty(orderType))
                {
                    // For events that don't support OrderType or no specific OrderType, find the first match (usually "ALL")
                    eventToUpdate = eventsList.FirstOrDefault(e => 
                        e.TryGetValue("Event", out var nameObj) && 
                        nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true &&
                        e.TryGetValue("OrderType", out var orderTypeObj) && 
                        orderTypeObj?.ToString()?.Equals("ALL", StringComparison.OrdinalIgnoreCase) == true);
                    
                    // If no "ALL" found, get the first match
                    if (eventToUpdate == null)
                    {
                        eventToUpdate = eventsList.FirstOrDefault(e => 
                            e.TryGetValue("Event", out var nameObj) && 
                            nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true);
                    }
                }
                else
                {
                    // Find the specific OrderType event to update
                    eventToUpdate = eventsList.FirstOrDefault(e => 
                        e.TryGetValue("Event", out var nameObj) && 
                        nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true &&
                        e.TryGetValue("OrderType", out var orderTypeObj) && 
                        orderTypeObj?.ToString()?.Equals(orderType, StringComparison.OrdinalIgnoreCase) == true);
                }

                if (eventToUpdate == null)
                    return false;

                // Update the event data
                foreach (var kvp in eventData)
                {
                    eventToUpdate[kvp.Key] = kvp.Value;
                }

                // Ensure the Event name and OrderType are correctly set
                eventToUpdate["Event"] = eventName;
                if (eventSupportsOrderType && !string.IsNullOrEmpty(orderType))
                {
                    eventToUpdate["OrderType"] = orderType;
                }

                // Update the Events array in the content
                existingContent["Events"] = eventsList;
                
                // Write back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                var updatedJson = JsonSerializer.Serialize(existingContent, options);
                await _fileContentService.WriteFileAsync(_notificationsFilePath, updatedJson);
                
                return true;
            }
            catch (Exception ex)
            {
                // Log error if logging is configured
                Console.WriteLine($"Error updating event by OrderType: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> GetActiveEventTriggersAsync()
        {
            try
            {
                var eventTriggersFilePath = "data/dictionaries/event-triggers.json";
                
                if (!await _fileContentService.FileExistsAsync(eventTriggersFilePath))
                {
                    return new List<string>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(eventTriggersFilePath);
                var triggers = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);
                
                if (triggers == null) return new List<string>();
                
                return triggers
                    .Where(t => t.ContainsKey("Event Name") && 
                               t.ContainsKey("IsActive") && 
                               t["IsActive"].ToString()?.ToLower() == "true")
                    .Select(t => t["Event Name"]?.ToString() ?? "")
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<Dictionary<string, object>?> GetEventTemplateAsync()
        {
            try
            {
                var templateFilePath = "data/templates/event-template.json";
                
                if (!await _fileContentService.FileExistsAsync(templateFilePath))
                {
                    return null;
                }

                var jsonContent = await _fileContentService.ReadFileAsync(templateFilePath);
                var template = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                
                return template;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddNewEventAsync(string eventName, EventMapping eventData)
        {
            var eventDict = new Dictionary<string, object>
            {
                ["Event"] = eventData.Event,
                ["OrderType"] = eventData.OrderType,
                ["Phone"] = eventData.Phone,
                ["Email"] = eventData.Email,
                ["Templates"] = eventData.Templates.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
                ["IsSuppressed"] = eventData.IsSuppressed,
                ["PreferredCommunication"] = eventData.PreferredCommunication,
                ["ContentVariables"] = eventData.ContentVariables,
                ["TriggerConditions"] = eventData.TriggerConditions,
                ["ContentVariablesOverrides"] = eventData.ContentVariablesOverrides
            };
            return await AddNewEventAsync(eventName, eventDict);
        }

        public async Task<bool> UpdateEventByOrderTypeAsync(string eventName, string? orderType, EventMapping eventData)
        {
            var eventDict = new Dictionary<string, object>
            {
                ["Event"] = eventData.Event,
                ["OrderType"] = eventData.OrderType,
                ["Phone"] = eventData.Phone,
                ["Email"] = eventData.Email,
                ["Templates"] = eventData.Templates.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value),
                ["IsSuppressed"] = eventData.IsSuppressed,
                ["PreferredCommunication"] = eventData.PreferredCommunication,
                ["ContentVariables"] = eventData.ContentVariables,
                ["TriggerConditions"] = eventData.TriggerConditions,
                ["ContentVariablesOverrides"] = eventData.ContentVariablesOverrides
            };
            return await UpdateEventByOrderTypeAsync(eventName, orderType, eventDict);
        }

        public async Task<bool> AddNewEventAsync(string eventName, Dictionary<string, object> eventData)
        {
            try
            {
                // Read the entire notifications.json file
                var existingContent = new Dictionary<string, object>();
                
                if (await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                    var document = JsonDocument.Parse(jsonContent);
                    
                    // Preserve existing sections
                    foreach (var prop in document.RootElement.EnumerateObject())
                    {
                        var deserializedValue = JsonSerializer.Deserialize<object>(prop.Value.GetRawText());
                        if (deserializedValue != null)
                        {
                            existingContent[prop.Name] = deserializedValue;
                        }
                    }
                }

                // Get existing Events array or create new one
                var eventsList = new List<Dictionary<string, object>>();
                if (existingContent.TryGetValue("Events", out var eventsObj) && eventsObj is JsonElement eventsElement)
                {
                    foreach (var item in eventsElement.EnumerateArray())
                    {
                        var eventDict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            eventDict[prop.Name] = ParseJsonPropertyValue(prop);
                        }
                        
                        // Only add events that don't match both the event name AND OrderType (to avoid duplicates)
                        var shouldKeepEvent = true;
                        
                        if (eventDict.TryGetValue("Event", out var existingEventName) && 
                            existingEventName?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            // Same event name - check if OrderType also matches
                            if (eventData.TryGetValue("OrderType", out var newOrderType) &&
                                eventDict.TryGetValue("OrderType", out var existingOrderType) &&
                                existingOrderType?.ToString()?.Equals(newOrderType?.ToString(), StringComparison.OrdinalIgnoreCase) == true)
                            {
                                // Same name and same OrderType - this is a duplicate, don't keep it
                                shouldKeepEvent = false;
                            }
                        }
                        
                        if (shouldKeepEvent)
                        {
                            eventsList.Add(eventDict);
                        }
                    }
                }

                // Add the new event with the Event parameter first
                var newEvent = new Dictionary<string, object>
                {
                    ["Event"] = eventName
                };
                
                // Add all other properties from eventData (excluding Event to avoid duplication)
                foreach (var kvp in eventData)
                {
                    if (!kvp.Key.Equals("Event", StringComparison.OrdinalIgnoreCase))
                    {
                        newEvent[kvp.Key] = kvp.Value;
                    }
                }
                
                eventsList.Add(newEvent);

                // Update the Events section
                existingContent["Events"] = eventsList;
                
                // Write back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                var updatedJson = JsonSerializer.Serialize(existingContent, options);
                await _fileContentService.WriteFileAsync(_notificationsFilePath, updatedJson);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<string>> GetAvailableOrderTypesAsync()
        {
            try
            {
                var orderTypesFilePath = "data/dictionaries/order-types.json";
                
                if (!await _fileContentService.FileExistsAsync(orderTypesFilePath))
                {
                    return new List<string>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(orderTypesFilePath);
                var orderTypes = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);
                
                if (orderTypes == null) return new List<string>();
                
                return orderTypes
                    .Where(ot => ot.ContainsKey("Order Type"))
                    .Select(ot => ot["Order Type"]?.ToString() ?? "")
                    .Where(orderType => !string.IsNullOrWhiteSpace(orderType))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<(string TemplateId, string TemplateName, string ChannelType)>> GetAvailableTemplatesAsync()
        {
            try
            {
                var templatesFilePath = "data/dictionaries/templates.json";
                
                if (!await _fileContentService.FileExistsAsync(templatesFilePath))
                {
                    return new List<(string, string, string)>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(templatesFilePath);
                var templates = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);
                
                if (templates == null) return new List<(string, string, string)>();
                
                return templates
                    .Where(t => t.ContainsKey("templateId") && t.ContainsKey("templateName") && t.ContainsKey("channelType"))
                    .Select(t => (
                        TemplateId: t["templateId"]?.ToString() ?? "",
                        TemplateName: t["templateName"]?.ToString() ?? "",
                        ChannelType: t["channelType"]?.ToString() ?? ""
                    ))
                    .Where(template => !string.IsNullOrWhiteSpace(template.TemplateId) && 
                                     !string.IsNullOrWhiteSpace(template.TemplateName) &&
                                     !string.IsNullOrWhiteSpace(template.ChannelType))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<(string, string, string)>();
            }
        }
        
        public async Task<Dictionary<string, string>> GetGlobalContentVariablesAsync()
        {
            try
            {
                if (!await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    return new Dictionary<string, string>();
                }

                var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
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
        
        public async Task<bool> UpdateEventContentVariablesAsync(string eventName, Dictionary<string, string> contentVariables)
        {
            try
            {
                if (!await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    return false;
                }

                var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                
                if (data == null) return false;

                // Get or create Events array
                if (!data.TryGetValue("Events", out var eventsObj) || eventsObj is not JsonElement eventsElement)
                {
                    return false;
                }

                var eventsList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(eventsElement.GetRawText());
                if (eventsList == null) return false;

                // Find the event
                var eventData = eventsList.FirstOrDefault(e => 
                    e.TryGetValue("Event", out var eventNameObj) && 
                    eventNameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true);

                if (eventData == null) return false;

                // Update or add ContentVariables
                if (contentVariables.Any())
                {
                    eventData["ContentVariables"] = contentVariables;
                }
                else
                {
                    // Remove ContentVariables if empty
                    eventData.Remove("ContentVariables");
                }

                // Update the Events array in the data
                data["Events"] = eventsList;

                // Save back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var updatedJson = JsonSerializer.Serialize(data, options);
                await _fileContentService.WriteFileAsync(_notificationsFilePath, updatedJson);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEventContentVariablesByOrderTypeAsync(string eventName, string? orderType, Dictionary<string, string> contentVariables)
        {
            try
            {
                if (!await _fileContentService.FileExistsAsync(_notificationsFilePath))
                {
                    return false;
                }

                var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                
                if (data == null) return false;

                // Get or create Events array
                if (!data.TryGetValue("Events", out var eventsObj) || eventsObj is not JsonElement eventsElement)
                {
                    return false;
                }

                var eventsList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(eventsElement.GetRawText());
                if (eventsList == null) return false;

                // Check if this event supports ByOrderType
                var eventSupportsOrderType = await CheckEventSupportsByOrderTypeAsync(eventName);

                // Find the event to update
                Dictionary<string, object>? eventData = null;
                
                if (!eventSupportsOrderType || string.IsNullOrEmpty(orderType))
                {
                    // For events that don't support OrderType or no specific OrderType, find the first match (usually "ALL")
                    eventData = eventsList.FirstOrDefault(e => 
                        e.TryGetValue("Event", out var nameObj) && 
                        nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true &&
                        e.TryGetValue("OrderType", out var orderTypeObj) && 
                        orderTypeObj?.ToString()?.Equals("ALL", StringComparison.OrdinalIgnoreCase) == true);
                    
                    // If no "ALL" found, get the first match
                    if (eventData == null)
                    {
                        eventData = eventsList.FirstOrDefault(e => 
                            e.TryGetValue("Event", out var nameObj) && 
                            nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true);
                    }
                }
                else
                {
                    // Find the specific OrderType event to update
                    eventData = eventsList.FirstOrDefault(e => 
                        e.TryGetValue("Event", out var nameObj) && 
                        nameObj?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true &&
                        e.TryGetValue("OrderType", out var orderTypeObj) && 
                        orderTypeObj?.ToString()?.Equals(orderType, StringComparison.OrdinalIgnoreCase) == true);
                }

                if (eventData == null) return false;

                // Update or add ContentVariables
                if (contentVariables.Any())
                {
                    eventData["ContentVariables"] = contentVariables;
                }
                else
                {
                    // Remove ContentVariables if empty
                    eventData.Remove("ContentVariables");
                }

                // Update the Events array in the data
                data["Events"] = eventsList;

                // Save back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var updatedJson = JsonSerializer.Serialize(data, options);
                await _fileContentService.WriteFileAsync(_notificationsFilePath, updatedJson);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<(string TemplateId, string TemplateName)>> GetAvailableTemplatesByChannelAsync(string channelType)
        {
            var allTemplates = await GetAvailableTemplatesAsync();
            
            return allTemplates
                .Where(template => template.ChannelType.Equals(channelType, StringComparison.OrdinalIgnoreCase))
                .Select(template => (template.TemplateId, template.TemplateName))
                .ToList();
        }

        public async Task<List<(string TemplateId, string TemplateName)>> GetEmailTemplatesAsync()
        {
            return await GetAvailableTemplatesByChannelAsync("Email");
        }

        public async Task<List<(string TemplateId, string TemplateName)>> GetSmsTemplatesAsync()
        {
            return await GetAvailableTemplatesByChannelAsync("SMS");
        }

        public async Task<List<(string TemplateId, string TemplateName)>> GetVoiceTemplatesAsync()
        {
            return await GetAvailableTemplatesByChannelAsync("Voice");
        }

        // Helper method to properly parse JSON property values and normalize Templates/ContentVariables
        private object ParseJsonPropertyValue(JsonProperty prop)
        {
            try
            {
                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        var stringValue = prop.Value.GetString() ?? "";
                        
                        // Special handling for Templates and ContentVariables that might be JSON strings
                        if ((prop.Name == "Templates" || prop.Name == "ContentVariables") && 
                            !string.IsNullOrEmpty(stringValue) && 
                            (stringValue.StartsWith("{") || stringValue.StartsWith("[")))
                        {
                            try
                            {
                                // Try to parse as JSON object/array
                                var parsed = JsonSerializer.Deserialize<object>(stringValue);
                                return parsed ?? stringValue;
                            }
                            catch (JsonException)
                            {
                                // If parsing fails, return as string
                                return stringValue;
                            }
                        }
                        return stringValue;
                        
                    case JsonValueKind.Number:
                        return prop.Value.TryGetInt32(out var intValue) ? intValue : prop.Value.GetDouble();
                        
                    case JsonValueKind.True:
                        return true;
                        
                    case JsonValueKind.False:
                        return false;
                        
                    case JsonValueKind.Null:
                        return "";
                        
                    case JsonValueKind.Object:
                        // Parse object to Dictionary<string, object>
                        var objDict = new Dictionary<string, object>();
                        foreach (var subProp in prop.Value.EnumerateObject())
                        {
                            objDict[subProp.Name] = ParseJsonPropertyValue(subProp);
                        }
                        return objDict;
                        
                    case JsonValueKind.Array:
                        // Parse array to List<object>
                        var list = new List<object>();
                        foreach (var item in prop.Value.EnumerateArray())
                        {
                            list.Add(JsonSerializer.Deserialize<object>(item.GetRawText()) ?? "");
                        }
                        return list;
                        
                    default:
                        return prop.Value.GetRawText();
                }
            }
            catch (Exception)
            {
                // Fallback to raw text if anything fails
                return prop.Value.GetRawText();
            }
        }
    }
}
