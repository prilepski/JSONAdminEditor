using System.Collections.Concurrent;
using System.Text.Json;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Services;

public interface IMockDatabaseService
{
    Task<CustomerNotificationMapping?> GetCustomerAsync(string customerId);
    Task<bool> SaveCustomerAsync(string customerId, CustomerNotificationMapping customerData);
    Task<List<string>> GetCustomerIdsAsync();
    Task<List<Customer>> SearchCustomersAsync(string searchTerm);
    Task<List<Template>> GetTemplatesAsync();
    Task<bool> SaveTemplatesAsync(List<Template> templates);
    Task<List<EventTrigger>> GetEventTriggersAsync();
    Task<bool> SaveEventTriggersAsync(List<EventTrigger> eventTriggers);
    Task<List<EventChannel>> GetEventChannelsAsync();
    Task<bool> SaveEventChannelsAsync(List<EventChannel> eventChannels);
    Task<List<OrderType>> GetOrderTypesAsync();
    Task<bool> SaveOrderTypesAsync(List<OrderType> orderTypes);
    Task<List<Customer>> GetCustomersAsync();
    Task<bool> SaveCustomersAsync(List<Customer> customers);
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
    private readonly List<Template> _templates = new();
    private readonly List<EventTrigger> _eventTriggers = new();
    private readonly List<EventChannel> _eventChannels = new();
    private readonly List<OrderType> _orderTypes = new();
    private readonly List<Customer> _customerList = new();
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

        InitializeReferenceData();
        InitializeGlobalData();
    }

    private void InitializeReferenceData()
    {
        _templates.AddRange(new[]
        {
            new Template { TemplateId = "d-e162b3e6181545fead5e643982628504", TemplateName = "Ready For Scheduling - Pickup", ChannelType = "Email" },
            new Template { TemplateId = "HX61a3cb19e35c428d9b1395d8139e94cc", TemplateName = "Ready For Scheduling - Pickup", ChannelType = "Sms" },
            new Template { TemplateId = "d-48388ac5bd1a4460969ac2ed36c818cc", TemplateName = "Ready For Scheduling - Delivery", ChannelType = "Email" },
            new Template { TemplateId = "Hxad1da069cc42534183daccd644579207", TemplateName = "Ready For Scheduling - Delivery", ChannelType = "Sms" }
        });

        _eventTriggers.AddRange(new[]
        {
            new EventTrigger { EventName = "Ready For Scheduling", IsActive = true },
            new EventTrigger { EventName = "Appointment Scheduled", IsActive = true },
            new EventTrigger { EventName = "Next Stop Update", IsActive = false }
        });

        _eventChannels.AddRange(new[]
        {
            new EventChannel { ChannelName = "Email", IsActive = true },
            new EventChannel { ChannelName = "SMS", IsActive = true },
            new EventChannel { ChannelName = "Voice", IsActive = false }
        });

        _orderTypes.AddRange(new[]
        {
            new OrderType { Name = "Delivery" },
            new OrderType { Name = "Pickup" }
        });

        _customerList.AddRange(new[]
        {
            new Customer { CustomerId = "CUST001", CompanyName = "DELL", ContactPerson = "John Doe", Email = "john@dell.com", Phone = "+1-555-0123", Address = "123 Dell Way", IsActive = true },
            new Customer { CustomerId = "CUST002", CompanyName = "Sample Corp", ContactPerson = "Jane Smith", Email = "jane@sample.com", Phone = "+1-555-0456", Address = "456 Sample St", IsActive = true },
            new Customer { CustomerId = "BJ001", CompanyName = "BJ Industries", ContactPerson = "Bob Johnson", Email = "bob@bj.com", Phone = "+1-555-0789", Address = "789 BJ Blvd", IsActive = true }
        });
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

    public Task<List<Customer>> SearchCustomersAsync(string searchTerm)
    {
        var results = _customerList
            .Where(c => c.CompanyName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       c.CustomerId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Task.FromResult(results);
    }

    public Task<List<Template>> GetTemplatesAsync()
    {
        return Task.FromResult(_templates.ToList());
    }

    public Task<bool> SaveTemplatesAsync(List<Template> templates)
    {
        _templates.Clear();
        _templates.AddRange(templates);
        return Task.FromResult(true);
    }

    public Task<List<EventTrigger>> GetEventTriggersAsync()
    {
        return Task.FromResult(_eventTriggers.ToList());
    }

    public Task<bool> SaveEventTriggersAsync(List<EventTrigger> eventTriggers)
    {
        _eventTriggers.Clear();
        _eventTriggers.AddRange(eventTriggers);
        return Task.FromResult(true);
    }

    public Task<List<EventChannel>> GetEventChannelsAsync()
    {
        return Task.FromResult(_eventChannels.ToList());
    }

    public Task<bool> SaveEventChannelsAsync(List<EventChannel> eventChannels)
    {
        _eventChannels.Clear();
        _eventChannels.AddRange(eventChannels);
        return Task.FromResult(true);
    }

    public Task<List<OrderType>> GetOrderTypesAsync()
    {
        return Task.FromResult(_orderTypes.ToList());
    }

    public Task<bool> SaveOrderTypesAsync(List<OrderType> orderTypes)
    {
        _orderTypes.Clear();
        _orderTypes.AddRange(orderTypes);
        return Task.FromResult(true);
    }

    public Task<List<Customer>> GetCustomersAsync()
    {
        return Task.FromResult(_customerList.ToList());
    }

    public Task<bool> SaveCustomersAsync(List<Customer> customers)
    {
        _customerList.Clear();
        _customerList.AddRange(customers);
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