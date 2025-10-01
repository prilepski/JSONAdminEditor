using System.Text.Json.Serialization;

namespace JSONAdminEditor.Domain.Entities;

public class CustomerNotificationMapping
{
    public List<PreferredCommunication> PreferredCommunication { get; set; } = new();

    public Dictionary<string, string> ContentVariables { get; set; } = new();

    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ContentVariablesOverrides { get; set; } = [];

    [JsonPropertyName("Events")]
    public List<CustomerEventMapping> EventMappings { get; set; } = new();

    public AfterHours? AfterHours { get; set; }

    public string? FromEmail { get; set; }
}
