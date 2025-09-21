namespace JSONAdminEditor.Application.Models;

public class CustomerNotificationMapping
{
    public List<EventMapping> EventMappings { get; set; } = new();
    public Dictionary<string, string> ContentVariables { get; set; } = new();
    public List<PreferredCommunication> PreferredCommunication { get; set; } = new();
    public AfterHours? AfterHours { get; set; }
    public string? FromEmail { get; set; }
}