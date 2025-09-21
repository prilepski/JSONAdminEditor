namespace JSONAdminEditor.Application.Models;

public class Template
{
    public string TemplateId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string ChannelType { get; set; } = string.Empty;
}

public class EventTrigger
{
    public string EventName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool ByOrderType { get; set; }
}

public class EventChannel
{
    public string ChannelName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class OrderType
{
    public string Name { get; set; } = string.Empty;
}

public class Customer
{
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}