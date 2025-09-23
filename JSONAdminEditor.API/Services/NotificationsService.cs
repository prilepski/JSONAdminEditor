using System.Text.Json;
using JSONAdminEditor.Application.Models.Configuration;
using JSONAdminEditor.Application.Models.Dictionaries;

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

        private async Task<NotificationMapping> LoadNotificationMappingAsync()
        {
            if (!await _fileContentService.FileExistsAsync(_notificationsFilePath))
            {
                return new NotificationMapping();
            }

            var jsonContent = await _fileContentService.ReadFileAsync(_notificationsFilePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<NotificationMapping>(jsonContent, options) ?? new NotificationMapping();
        }

        private async Task SaveNotificationMappingAsync(NotificationMapping mapping)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonContent = JsonSerializer.Serialize(mapping, options);
            await _fileContentService.WriteFileAsync(_notificationsFilePath, jsonContent);
        }

        public async Task<List<PreferredCommunication>> GetPreferredCommunicationAsync()
        {
            var mapping = await LoadNotificationMappingAsync();
            return mapping.PreferredCommunication;
        }

        public async Task<bool> UpdatePreferredCommunicationAsync(List<PreferredCommunication> preferredCommunication)
        {
            try
            {
                var mapping = await LoadNotificationMappingAsync();
                mapping.PreferredCommunication = preferredCommunication;
                await SaveNotificationMappingAsync(mapping);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetContentVariablesAsync()
        {
            var mapping = await LoadNotificationMappingAsync();
            return mapping.ContentVariables;
        }

        public async Task<bool> UpdateContentVariablesAsync(Dictionary<string, string> contentVariables)
        {
            try
            {
                var mapping = await LoadNotificationMappingAsync();
                mapping.ContentVariables = contentVariables;
                await SaveNotificationMappingAsync(mapping);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<EventMapping>> GetEventsAsync()
        {
            var mapping = await LoadNotificationMappingAsync();
            return mapping.EventMappings;
        }

        public async Task<EventMapping?> GetEventByNameAsync(string eventName)
        {
            var events = await GetEventsAsync();
            return events.FirstOrDefault(e => e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<EventMapping?> GetEventByNameAndOrderTypeAsync(string eventName, string? orderType = null)
        {
            var events = await GetEventsAsync();
            var eventSupportsOrderType = await CheckEventSupportsByOrderTypeAsync(eventName);

            var matchingEvents = events.Where(e => e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!matchingEvents.Any()) return null;

            if (!eventSupportsOrderType)
            {
                return matchingEvents.FirstOrDefault(e => e.OrderType.Equals("ALL", StringComparison.OrdinalIgnoreCase)) 
                    ?? matchingEvents.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(orderType))
            {
                return matchingEvents.FirstOrDefault(e => e.OrderType.Equals("ALL", StringComparison.OrdinalIgnoreCase)) 
                    ?? matchingEvents.FirstOrDefault();
            }

            return matchingEvents.FirstOrDefault(e => e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> AddNewEventAsync(string eventName, EventMapping eventData)
        {
            try
            {
                var mapping = await LoadNotificationMappingAsync();
                
                // Remove existing events with same name and order type
                mapping.EventMappings.RemoveAll(e => 
                    e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
                    e.OrderType.Equals(eventData.OrderType, StringComparison.OrdinalIgnoreCase));
                
                mapping.EventMappings.Add(eventData);
                await SaveNotificationMappingAsync(mapping);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEventByOrderTypeAsync(string eventName, string? orderType, EventMapping eventData)
        {
            try
            {
                var mapping = await LoadNotificationMappingAsync();
                var eventSupportsOrderType = await CheckEventSupportsByOrderTypeAsync(eventName);

                EventMapping? eventToUpdate = null;

                if (!eventSupportsOrderType || string.IsNullOrEmpty(orderType))
                {
                    eventToUpdate = mapping.EventMappings.FirstOrDefault(e => 
                        e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
                        e.OrderType.Equals("ALL", StringComparison.OrdinalIgnoreCase));
                    
                    if (eventToUpdate == null)
                    {
                        eventToUpdate = mapping.EventMappings.FirstOrDefault(e => 
                            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase));
                    }
                }
                else
                {
                    eventToUpdate = mapping.EventMappings.FirstOrDefault(e => 
                        e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
                        e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));
                }

                if (eventToUpdate == null) return false;

                // Update properties
                eventToUpdate.Phone = eventData.Phone;
                eventToUpdate.Email = eventData.Email;
                eventToUpdate.Templates = eventData.Templates;
                eventToUpdate.IsSuppressed = eventData.IsSuppressed;
                eventToUpdate.PreferredCommunication = eventData.PreferredCommunication;
                eventToUpdate.ContentVariables = eventData.ContentVariables;
                eventToUpdate.TriggerConditions = eventData.TriggerConditions;

                await SaveNotificationMappingAsync(mapping);
                return true;
            }
            catch
            {
                return false;
            }
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
                var triggers = JsonSerializer.Deserialize<List<EventTrigger>>(jsonContent);
                
                if (triggers == null) return false;
                
                var eventTrigger = triggers.FirstOrDefault(t => 
                    t.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase));

                return eventTrigger?.ByOrderType ?? false;
            }
            catch
            {
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
                var triggers = JsonSerializer.Deserialize<List<EventTrigger>>(jsonContent);
                
                if (triggers == null) return new List<string>();
                
                return triggers
                    .Where(t => t.IsActive)
                    .Select(t => t.EventName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<EventMapping?> GetEventTemplateAsync()
        {
            try
            {
                var templateFilePath = "data/templates/event-template.json";
                
                if (!await _fileContentService.FileExistsAsync(templateFilePath))
                {
                    return null;
                }

                var jsonContent = await _fileContentService.ReadFileAsync(templateFilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<EventMapping>(jsonContent, options);
            }
            catch
            {
                return null;
            }
        }
    }
}