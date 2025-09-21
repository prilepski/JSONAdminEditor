using System.Collections.Concurrent;
using System.Text.Json;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Services;

public interface IMockDatabaseService
{
    Task<CustomerNotificationMapping?> GetCustomerAsync(string customerId);
    Task<bool> SaveCustomerAsync(string customerId, CustomerNotificationMapping customerData);
    Task<List<string>> GetCustomerIdsAsync();
    Task<List<Dictionary<string, object>>> SearchCustomersAsync(string searchTerm);
    Task<List<Dictionary<string, object>>?> GetDictionaryAsync(string dictionaryType);
    Task<bool> SaveDictionaryAsync(string dictionaryType, List<Dictionary<string, object>> data);
    Task<NotificationMapping> GetNotificationMappingAsync();
    Task<bool> SaveNotificationMappingAsync(NotificationMapping notificationMapping);
    Task<OptOut> GetOptOutAsync();
    Task<bool> SaveOptOutAsync(OptOut optOut);
    Task<AfterHours?> GetAfterHoursAsync();
    Task<bool> SaveAfterHoursAsync(AfterHours afterHours);
    Task<List<PreferredCommunication>> GetPreferredCommunicationAsync();
    Task<bool> SavePreferredCommunicationAsync(List<PreferredCommunication> preferredCommunication);
    Task<Dictionary<string, string>> GetContentVariablesAsync();
    Task<bool> SaveContentVariablesAsync(Dictionary<string, string> contentVariables);
}

public class MockDatabaseService : IMockDatabaseService
{
    private readonly ConcurrentDictionary<string, CustomerNotificationMapping> _customers = new();
    private readonly ConcurrentDictionary<string, List<Dictionary<string, object>>> _dictionaries = new();
    private readonly NotificationMapping _globalData = new();
    private readonly ILogger<MockDatabaseService> _logger;

    public MockDatabaseService(ILogger<MockDatabaseService> logger)
    {
        _logger = logger;
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        // CUST001 - DELL (minimal overrides)
        _customers["CUST001"] = new CustomerNotificationMapping
        {
            EventMappings = new List<EventMapping>(),
            ContentVariables = new Dictionary<string, string>()
        };

        // CUST002 - Sample Customer (with overrides)
        _customers["CUST002"] = new CustomerNotificationMapping
        {
            ContentVariables = new Dictionary<string, string>
            {
                ["cust_name"] = "$Customer.FriendlyName$",
                ["custom_var"] = "Sample Value"
            },
            EventMappings = new List<EventMapping>
            {
                new()
                {
                    Event = "Ready For Scheduling",
                    OrderType = "Delivery",
                    Phone = "$consignee.phone$",
                    Email = "$consignee.email$",
                    Templates = new Dictionary<Channel, string>
                    {
                        [Channel.Email] = "d-e162b3e6181545fead5e643982628504",
                        [Channel.Sms] = "HX61a3cb19e35c428d9b1395d8139e94cc"
                    }
                }
            },
            FromEmail = "notifications@samplecorp.com",
            PreferredCommunication = new List<PreferredCommunication>
            {
                new() { Channel = Channel.Sms, Priority = 1 },
                new() { Channel = Channel.Email, Priority = 2 }
            }
        };

        // BJ001 - Another sample (minimal overrides)
        _customers["BJ001"] = new CustomerNotificationMapping
        {
            EventMappings = new List<EventMapping>(),
            ContentVariables = new Dictionary<string, string>()
        };

        InitializeDictionaries();
        InitializeGlobalData();
    }

    private void InitializeDictionaries()
    {
        // Templates dictionary
        _dictionaries["templates"] = new List<Dictionary<string, object>>
        {
            new() { ["templateId"] = "d-e162b3e6181545fead5e643982628504", ["templateName"] = "Ready For Scheduling - Pickup", ["channelType"] = "Email" },
            new() { ["templateId"] = "HX61a3cb19e35c428d9b1395d8139e94cc", ["templateName"] = "Ready For Scheduling - Pickup", ["channelType"] = "Sms" },
            new() { ["templateId"] = "d-48388ac5bd1a4460969ac2ed36c818cc", ["templateName"] = "Ready For Scheduling - Delivery", ["channelType"] = "Email" },
            new() { ["templateId"] = "Hxad1da069cc42534183daccd644579207", ["templateName"] = "Ready For Scheduling - Delivery", ["channelType"] = "Sms" }
        };

        // Event triggers dictionary
        _dictionaries["event-triggers"] = new List<Dictionary<string, object>>
        {
            new() { ["Event Name"] = "Ready For Scheduling", ["IsActive"] = true },
            new() { ["Event Name"] = "Appointment Scheduled", ["IsActive"] = true },
            new() { ["Event Name"] = "Next Stop Update", ["IsActive"] = false }
        };

        // Event channels dictionary
        _dictionaries["event-channels"] = new List<Dictionary<string, object>>
        {
            new() { ["Channel Name"] = "Email", ["IsActive"] = true },
            new() { ["Channel Name"] = "SMS", ["IsActive"] = true },
            new() { ["Channel Name"] = "Voice", ["IsActive"] = false }
        };

        // Order types dictionary
        _dictionaries["order-types"] = new List<Dictionary<string, object>>
        {
            new() { ["Order Type"] = "Delivery" },
            new() { ["Order Type"] = "Pickup" }
        };

        // Customers dictionary
        _dictionaries["customers"] = new List<Dictionary<string, object>>
        {
            new() { ["customerId"] = "CUST001", ["companyName"] = "DELL", ["contactPerson"] = "John Doe", ["email"] = "john@dell.com", ["phone"] = "+1-555-0123", ["address"] = "123 Dell Way", ["IsActive"] = true },
            new() { ["customerId"] = "CUST002", ["companyName"] = "Sample Corp", ["contactPerson"] = "Jane Smith", ["email"] = "jane@sample.com", ["phone"] = "+1-555-0456", ["address"] = "456 Sample St", ["IsActive"] = true },
            new() { ["customerId"] = "BJ001", ["companyName"] = "BJ Industries", ["contactPerson"] = "Bob Johnson", ["email"] = "bob@bj.com", ["phone"] = "+1-555-0789", ["address"] = "789 BJ Blvd", ["IsActive"] = true }
        };
    }

    private void InitializeGlobalData()
    {
        _globalData.EventMappings = new List<EventMapping>
        {
            new() { Event = "Ready For Scheduling", OrderType = "Delivery", Phone = "$consignee.phone$", Email = "$consignee.email$", Templates = new Dictionary<Channel, string> { [Channel.Email] = "d-48388ac5bd1a4460969ac2ed36c818cc", [Channel.Sms] = "Hxad1da069cc42534183daccd644579207" } },
            new() { Event = "Ready For Scheduling", OrderType = "Pickup", Phone = "$consignee.phone$", Email = "$consignee.email$", Templates = new Dictionary<Channel, string> { [Channel.Email] = "d-e162b3e6181545fead5e643982628504", [Channel.Sms] = "HX61a3cb19e35c428d9b1395d8139e94cc" } },
            new() { Event = "Appointment Scheduled", OrderType = "ALL", Phone = "$consignee.phone$", Email = "$consignee.email$", Templates = new Dictionary<Channel, string> { [Channel.Email] = "d-bd0b88948dd34009b08434c16b76c1e0" } }
        };
        
        _globalData.PreferredCommunication = new List<PreferredCommunication>
        {
            new() { Channel = Channel.Email, Priority = 1 },
            new() { Channel = Channel.Sms, Priority = 2 }
        };
        
        _globalData.ContentVariables = new Dictionary<string, string>
        {
            ["company_name"] = "$Company.Name$",
            ["customer_phone"] = "$Customer.Phone$",
            ["order_id"] = "$Order.Id$",
            ["consignee_email"] = "$consignee.email$",
            ["consignee_phone"] = "$consignee.phone$"
        };
        
        _globalData.OptOut = new OptOut
        {
            ["en"] = new Dictionary<string, OptOutLocale>
            {
                ["US"] = new OptOutLocale
                {
                    OptOutKeywords = "STOP, QUIT, CANCEL",
                    OptInKeywords = "START, YES",
                    HelpKeywords = "HELP, INFO",
                    OptOutFooter = "Reply STOP to opt out",
                    OptOutPhrase = "You have been unsubscribed",
                    OptInPhrase = "You have been subscribed",
                    OptInMessage = "Welcome! You will receive notifications",
                    HelpPhrase = "For help, contact support"
                }
            }
        };
        
        _globalData.AfterHours = new AfterHours
        {
            RestrictedHoursPeriod = new RestrictedHoursPeriod
            {
                Start = "22:00",
                End = "08:00"
            },
            ExceptionOfValidation = new ExceptionOfValidation
            {
                Events = new List<EventBase>
                {
                    new() { Name = "Emergency Alert", Type = "Critical" }
                }
            }
        };
        
        _globalData.FromEmail = "noreply@company.com";
    }

    public Task<CustomerNotificationMapping?> GetCustomerAsync(string customerId)
    {
        _customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }

    public Task<bool> SaveCustomerAsync(string customerId, CustomerNotificationMapping customerData)
    {
        _customers[customerId] = customerData;
        return Task.FromResult(true);
    }

    public Task<List<string>> GetCustomerIdsAsync()
    {
        return Task.FromResult(_customers.Keys.ToList());
    }

    public Task<List<Dictionary<string, object>>> SearchCustomersAsync(string searchTerm)
    {
        var results = _dictionaries["customers"]
            .Where(c => c["companyName"].ToString()!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       c["customerId"].ToString()!.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Task.FromResult(results);
    }

    public Task<List<Dictionary<string, object>>?> GetDictionaryAsync(string dictionaryType)
    {
        _dictionaries.TryGetValue(dictionaryType, out var dictionary);
        return Task.FromResult(dictionary);
    }

    public Task<bool> SaveDictionaryAsync(string dictionaryType, List<Dictionary<string, object>> data)
    {
        _dictionaries[dictionaryType] = data;
        return Task.FromResult(true);
    }

    public Task<NotificationMapping> GetNotificationMappingAsync()
    {
        return Task.FromResult(_globalData);
    }

    public Task<bool> SaveNotificationMappingAsync(NotificationMapping notificationMapping)
    {
        _globalData.EventMappings = notificationMapping.EventMappings;
        _globalData.PreferredCommunication = notificationMapping.PreferredCommunication;
        _globalData.ContentVariables = notificationMapping.ContentVariables;
        _globalData.OptOut = notificationMapping.OptOut;
        _globalData.AfterHours = notificationMapping.AfterHours;
        _globalData.FromEmail = notificationMapping.FromEmail;
        _globalData.Agents = notificationMapping.Agents;
        return Task.FromResult(true);
    }

    public Task<OptOut> GetOptOutAsync()
    {
        return Task.FromResult(_globalData.OptOut);
    }

    public Task<bool> SaveOptOutAsync(OptOut optOut)
    {
        _globalData.OptOut = optOut;
        return Task.FromResult(true);
    }

    public Task<AfterHours?> GetAfterHoursAsync()
    {
        return Task.FromResult(_globalData.AfterHours);
    }

    public Task<bool> SaveAfterHoursAsync(AfterHours afterHours)
    {
        _globalData.AfterHours = afterHours;
        return Task.FromResult(true);
    }

    public Task<List<PreferredCommunication>> GetPreferredCommunicationAsync()
    {
        return Task.FromResult(_globalData.PreferredCommunication);
    }

    public Task<bool> SavePreferredCommunicationAsync(List<PreferredCommunication> preferredCommunication)
    {
        _globalData.PreferredCommunication = preferredCommunication;
        return Task.FromResult(true);
    }

    public Task<Dictionary<string, string>> GetContentVariablesAsync()
    {
        return Task.FromResult(_globalData.ContentVariables);
    }

    public Task<bool> SaveContentVariablesAsync(Dictionary<string, string> contentVariables)
    {
        _globalData.ContentVariables = contentVariables;
        return Task.FromResult(true);
    }
}