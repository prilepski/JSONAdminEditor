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
    }
}
