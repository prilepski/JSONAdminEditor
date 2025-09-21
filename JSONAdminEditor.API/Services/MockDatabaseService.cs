using System.Collections.Concurrent;
using System.Text.Json;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Services;

public interface IMockDatabaseService
{
    Task<Dictionary<string, object>?> GetCustomerAsync(string customerId);
    Task<bool> SaveCustomerAsync(string customerId, Dictionary<string, object> customerData);
    Task<List<string>> GetCustomerIdsAsync();
    Task<List<Dictionary<string, object>>> SearchCustomersAsync(string searchTerm);
    Task<List<Dictionary<string, object>>?> GetDictionaryAsync(string dictionaryType);
    Task<bool> SaveDictionaryAsync(string dictionaryType, List<Dictionary<string, object>> data);
    Task<Dictionary<string, object>?> GetGlobalDataAsync(string key);
    Task<bool> SaveGlobalDataAsync(string key, Dictionary<string, object> data);
    Task<T?> GetGlobalDataAsync<T>(string key) where T : class;
    Task<bool> SaveGlobalDataAsync<T>(string key, T data) where T : class;
}

public class MockDatabaseService : IMockDatabaseService
{
    private readonly ConcurrentDictionary<string, Dictionary<string, object>> _customers = new();
    private readonly ConcurrentDictionary<string, List<Dictionary<string, object>>> _dictionaries = new();
    private readonly Dictionary<string, object> _globalData = new();
    private readonly ILogger<MockDatabaseService> _logger;

    public MockDatabaseService(ILogger<MockDatabaseService> logger)
    {
        _logger = logger;
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        // CUST001 - DELL
        _customers["CUST001"] = new Dictionary<string, object>
        {
            ["customerName"] = "DELL",
            ["Events"] = new List<Dictionary<string, object>>(),
            ["ContentVariables"] = new Dictionary<string, object>()
        };

        // CUST002 - Sample Customer
        _customers["CUST002"] = new Dictionary<string, object>
        {
            ["customerName"] = "Sample Corp",
            ["ContentVariables"] = new Dictionary<string, object>
            {
                ["cust_name"] = "$Customer.FriendlyName$",
                ["custom_var"] = "Sample Value"
            },
            ["Events"] = new List<Dictionary<string, object>>
            {
                new()
                {
                    ["Event"] = "Ready For Scheduling",
                    ["OrderType"] = "Delivery",
                    ["Phone"] = "$consignee.phone$",
                    ["Email"] = "$consignee.email$",
                    ["Templates"] = new Dictionary<string, object>
                    {
                        ["Email"] = "d-e162b3e6181545fead5e643982628504",
                        ["Sms"] = "HX61a3cb19e35c428d9b1395d8139e94cc"
                    }
                }
            }
        };

        // BJ001 - Another sample
        _customers["BJ001"] = new Dictionary<string, object>
        {
            ["customerName"] = "BJ Industries",
            ["Events"] = new List<Dictionary<string, object>>(),
            ["ContentVariables"] = new Dictionary<string, object>()
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
        // Content variables
        _globalData["content-variables"] = new Dictionary<string, object>
        {
            ["data"] = new List<Dictionary<string, object>>
            {
                new() { ["Variable Name"] = "company_name", ["Variable Value"] = "$Company.Name$", ["Description"] = "Company name placeholder" },
                new() { ["Variable Name"] = "customer_phone", ["Variable Value"] = "$Customer.Phone$", ["Description"] = "Customer phone number" },
                new() { ["Variable Name"] = "order_id", ["Variable Value"] = "$Order.Id$", ["Description"] = "Order identifier" },
                new() { ["Variable Name"] = "consignee_email", ["Variable Value"] = "$consignee.email$", ["Description"] = "Consignee email address" },
                new() { ["Variable Name"] = "consignee_phone", ["Variable Value"] = "$consignee.phone$", ["Description"] = "Consignee phone number" }
            }
        };

        // Preferred communication
        _globalData["preferred-communication"] = new Dictionary<string, object>
        {
            ["data"] = new List<Dictionary<string, object>>
            {
                new() { ["Customer ID"] = "CUST001", ["Preferred Channel"] = "Email", ["Phone"] = "+1-555-0123", ["Email"] = "customer1@example.com", ["IsActive"] = true },
                new() { ["Customer ID"] = "CUST002", ["Preferred Channel"] = "SMS", ["Phone"] = "+1-555-0456", ["Email"] = "customer2@example.com", ["IsActive"] = true },
                new() { ["Customer ID"] = "BJ001", ["Preferred Channel"] = "Email", ["Phone"] = "+1-555-0789", ["Email"] = "customer3@example.com", ["IsActive"] = false }
            }
        };

        // NotificationMapping global data
        var notificationMapping = new NotificationMapping
        {
            EventMappings = new List<EventMapping>
            {
                new() { Event = "Ready For Scheduling", OrderType = "Delivery", Phone = "$consignee.phone$", Email = "$consignee.email$", Templates = new Dictionary<Channel, string> { [Channel.Email] = "d-48388ac5bd1a4460969ac2ed36c818cc", [Channel.Sms] = "Hxad1da069cc42534183daccd644579207" } },
                new() { Event = "Ready For Scheduling", OrderType = "Pickup", Phone = "$consignee.phone$", Email = "$consignee.email$", Templates = new Dictionary<Channel, string> { [Channel.Email] = "d-e162b3e6181545fead5e643982628504", [Channel.Sms] = "HX61a3cb19e35c428d9b1395d8139e94cc" } },
                new() { Event = "Appointment Scheduled", OrderType = "ALL", Phone = "$consignee.phone$", Email = "$consignee.email$", Templates = new Dictionary<Channel, string> { [Channel.Email] = "d-bd0b88948dd34009b08434c16b76c1e0" } }
            },
            FromEmail = "noreply@company.com"
        };
        _globalData["events"] = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(notificationMapping)) ?? new Dictionary<string, object>();

        // OptOut global data
        _globalData["optout"] = new Dictionary<string, object>
        {
            ["data"] = new Dictionary<string, Dictionary<string, object>>
            {
                ["en"] = new Dictionary<string, object>
                {
                    ["US"] = new Dictionary<string, object>
                    {
                        ["optOutKeywords"] = "STOP, QUIT, CANCEL",
                        ["optInKeywords"] = "START, YES",
                        ["helpKeywords"] = "HELP, INFO",
                        ["optOutFooter"] = "Reply STOP to opt out",
                        ["optOutPhrase"] = "You have been unsubscribed",
                        ["optInPhrase"] = "You have been subscribed",
                        ["optInMessage"] = "Welcome! You will receive notifications",
                        ["helpPhrase"] = "For help, contact support"
                    }
                }
            }
        };

        // Global AfterHours data
        _globalData["afterhours"] = new Dictionary<string, object>
        {
            ["data"] = new Dictionary<string, object>
            {
                ["RestrictedHoursPeriod"] = new Dictionary<string, object>
                {
                    ["Start"] = "22:00",
                    ["End"] = "08:00"
                },
                ["RestrictedDays"] = new List<int> { 0, 6 }, // Sunday, Saturday
                ["ExceptionOfValidation"] = new Dictionary<string, object>
                {
                    ["Events"] = new List<Dictionary<string, object>>
                    {
                        new() { ["Name"] = "Emergency Alert", ["Type"] = "Critical" }
                    }
                }
            }
        };

        // NotificationMapping global data - use the same structure as events
        _globalData["notification-mapping"] = new Dictionary<string, object>
        {
            ["data"] = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(notificationMapping)) ?? new Dictionary<string, object>()
        };
    }

    public Task<Dictionary<string, object>?> GetCustomerAsync(string customerId)
    {
        _customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }

    public Task<bool> SaveCustomerAsync(string customerId, Dictionary<string, object> customerData)
    {
        _customers[customerId] = customerData;
        _logger.LogInformation("Saved customer {CustomerId}", customerId);
        return Task.FromResult(true);
    }

    public Task<List<string>> GetCustomerIdsAsync()
    {
        return Task.FromResult(_customers.Keys.ToList());
    }

    public Task<List<Dictionary<string, object>>> SearchCustomersAsync(string searchTerm)
    {
        var results = new List<Dictionary<string, object>>();
        
        foreach (var kvp in _customers)
        {
            var customerId = kvp.Key;
            var customerData = kvp.Value;
            var customerName = customerData.TryGetValue("customerName", out var name) ? name?.ToString() : customerId;
            
            if (customerId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (customerName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true))
            {
                results.Add(new Dictionary<string, object>
                {
                    ["customerId"] = customerId,
                    ["companyName"] = customerName ?? customerId,
                    ["contactPerson"] = "Contact Person",
                    ["email"] = $"{customerId.ToLower()}@company.com",
                    ["phone"] = "+1-555-0123",
                    ["address"] = "123 Business St",
                    ["IsActive"] = true
                });
            }
        }
        
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
        _logger.LogInformation("Saved dictionary {DictionaryType}", dictionaryType);
        return Task.FromResult(true);
    }

    public Task<Dictionary<string, object>?> GetGlobalDataAsync(string key)
    {
        _globalData.TryGetValue(key, out var data);
        return Task.FromResult(data as Dictionary<string, object>);
    }

    public Task<bool> SaveGlobalDataAsync(string key, Dictionary<string, object> data)
    {
        _globalData[key] = data;
        _logger.LogInformation("Saved global data {Key}", key);
        return Task.FromResult(true);
    }

    public Task<T?> GetGlobalDataAsync<T>(string key) where T : class
    {
        if (_globalData.TryGetValue(key, out var data))
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var result = JsonSerializer.Deserialize<T>(json);
                return Task.FromResult(result);
            }
            catch
            {
                return Task.FromResult<T?>(null);
            }
        }
        return Task.FromResult<T?>(null);
    }

    public Task<bool> SaveGlobalDataAsync<T>(string key, T data) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            if (dict != null)
            {
                _globalData[key] = dict;
                _logger.LogInformation("Saved global data {Key}", key);
                return Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save global data {Key}", key);
        }
        return Task.FromResult(false);
    }
}