using JSONAdminEditor.Models;
using System.Text.Json;

namespace JSONAdminEditor.Services
{
    public class NotificationsService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _notificationsFilePath;

        public NotificationsService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _notificationsFilePath = Path.Combine(_environment.WebRootPath, "data", "notifications.json");
        }

        public async Task<List<Dictionary<string, object>>?> GetPreferredCommunicationAsync()
        {
            try
            {
                if (!File.Exists(_notificationsFilePath))
                {
                    return new List<Dictionary<string, object>>();
                }

                var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
                var document = JsonDocument.Parse(jsonContent);
                
                if (document.RootElement.TryGetProperty("PreferredCommunication", out var prefCommElement))
                {
                    var result = new List<Dictionary<string, object>>();
                    
                    foreach (var item in prefCommElement.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            dict[prop.Name] = prop.Value.ValueKind switch
                            {
                                JsonValueKind.String => prop.Value.GetString() ?? "",
                                JsonValueKind.Number => prop.Value.GetInt32(),
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                _ => prop.Value.GetRawText()
                            };
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
                
                if (File.Exists(_notificationsFilePath))
                {
                    var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
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
                    PropertyNamingPolicy = null
                };
                
                var updatedJson = JsonSerializer.Serialize(existingContent, options);
                await File.WriteAllTextAsync(_notificationsFilePath, updatedJson);
                
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
                var channelsFilePath = Path.Combine(_environment.WebRootPath, "data", "event-channels.json");
                
                if (!File.Exists(channelsFilePath))
                {
                    return new List<string>();
                }

                var jsonContent = await File.ReadAllTextAsync(channelsFilePath);
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
                if (!File.Exists(_notificationsFilePath))
                {
                    return new List<Dictionary<string, object>>();
                }

                var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
                var document = JsonDocument.Parse(jsonContent);
                
                if (document.RootElement.TryGetProperty("ContentVariables", out var contentVarsElement))
                {
                    var result = new List<Dictionary<string, object>>();
                    
                    foreach (var prop in contentVarsElement.EnumerateObject())
                    {
                        var dict = new Dictionary<string, object>
                        {
                            ["Variable Name"] = prop.Name,
                            ["Variable Mapping"] = prop.Value.ValueKind switch
                            {
                                JsonValueKind.String => prop.Value.GetString() ?? "",
                                JsonValueKind.Number => prop.Value.GetInt32(),
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                _ => prop.Value.GetRawText()
                            }
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
                
                if (File.Exists(_notificationsFilePath))
                {
                    var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
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
                    PropertyNamingPolicy = null
                };
                
                var updatedJson = JsonSerializer.Serialize(existingContent, options);
                await File.WriteAllTextAsync(_notificationsFilePath, updatedJson);
                
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
                if (!File.Exists(_notificationsFilePath))
                {
                    return new List<Dictionary<string, object>>();
                }

                var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
                var document = JsonDocument.Parse(jsonContent);
                
                if (document.RootElement.TryGetProperty("Events", out var eventsElement))
                {
                    var result = new List<Dictionary<string, object>>();
                    
                    foreach (var item in eventsElement.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            dict[prop.Name] = prop.Value.ValueKind switch
                            {
                                JsonValueKind.String => prop.Value.GetString() ?? "",
                                JsonValueKind.Number => prop.Value.GetInt32(),
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                JsonValueKind.Null => "",
                                _ => prop.Value.GetRawText()
                            };
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

        public async Task<bool> UpdateEventAsync(string eventName, Dictionary<string, object> eventData)
        {
            // Use the same logic as AddNewEventAsync - remove existing and add new
            return await AddNewEventAsync(eventName, eventData);
        }

        public async Task<List<string>> GetActiveEventTriggersAsync()
        {
            try
            {
                var eventTriggersFilePath = Path.Combine(_environment.WebRootPath, "data", "event-triggers.json");
                
                if (!File.Exists(eventTriggersFilePath))
                {
                    return new List<string>();
                }

                var jsonContent = await File.ReadAllTextAsync(eventTriggersFilePath);
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
                var templateFilePath = Path.Combine(_environment.WebRootPath, "data", "templates", "event-template.json");
                
                if (!File.Exists(templateFilePath))
                {
                    return null;
                }

                var jsonContent = await File.ReadAllTextAsync(templateFilePath);
                var template = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                
                return template;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddNewEventAsync(string eventName, Dictionary<string, object> eventData)
        {
            try
            {
                // Read the entire notifications.json file
                var existingContent = new Dictionary<string, object>();
                
                if (File.Exists(_notificationsFilePath))
                {
                    var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
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
                            eventDict[prop.Name] = prop.Value.ValueKind switch
                            {
                                JsonValueKind.String => prop.Value.GetString() ?? "",
                                JsonValueKind.Number => prop.Value.GetInt32(),
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                JsonValueKind.Null => "",
                                _ => prop.Value.GetRawText()
                            };
                        }
                        
                        // Only add events that don't match the new event name (remove duplicates)
                        if (eventDict.TryGetValue("Event", out var existingEventName) && 
                            !existingEventName?.ToString()?.Equals(eventName, StringComparison.OrdinalIgnoreCase) == true)
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
                    PropertyNamingPolicy = null
                };
                
                var updatedJson = JsonSerializer.Serialize(existingContent, options);
                await File.WriteAllTextAsync(_notificationsFilePath, updatedJson);
                
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
                var orderTypesFilePath = Path.Combine(_environment.WebRootPath, "data", "order-types.json");
                
                if (!File.Exists(orderTypesFilePath))
                {
                    return new List<string>();
                }

                var jsonContent = await File.ReadAllTextAsync(orderTypesFilePath);
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

        public async Task<List<(string TemplateId, string TemplateName)>> GetAvailableTemplatesAsync()
        {
            try
            {
                var templatesFilePath = Path.Combine(_environment.WebRootPath, "data", "templates.json");
                
                if (!File.Exists(templatesFilePath))
                {
                    return new List<(string, string)>();
                }

                var jsonContent = await File.ReadAllTextAsync(templatesFilePath);
                var templates = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);
                
                if (templates == null) return new List<(string, string)>();
                
                return templates
                    .Where(t => t.ContainsKey("templateId") && t.ContainsKey("templateName"))
                    .Select(t => (
                        TemplateId: t["templateId"]?.ToString() ?? "",
                        TemplateName: t["templateName"]?.ToString() ?? ""
                    ))
                    .Where(template => !string.IsNullOrWhiteSpace(template.TemplateId) && 
                                     !string.IsNullOrWhiteSpace(template.TemplateName))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<(string, string)>();
            }
        }
        
        public async Task<Dictionary<string, string>> GetGlobalContentVariablesAsync()
        {
            try
            {
                if (!File.Exists(_notificationsFilePath))
                {
                    return new Dictionary<string, string>();
                }

                var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
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
                if (!File.Exists(_notificationsFilePath))
                {
                    return false;
                }

                var jsonContent = await File.ReadAllTextAsync(_notificationsFilePath);
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
                await File.WriteAllTextAsync(_notificationsFilePath, updatedJson);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
