using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models.Structure;

public class NotificationMapping
{
    public List<PreferredCommunication> PreferredCommunication { get; set; } = [];

    public Dictionary<string, string> ContentVariables { get; set; } = [];

    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ContentVariablesOverrides { get; set; } = [];

    [JsonPropertyName("Events")]
    public List<EventMapping> EventMappings { get; set; } = [];

    public OptOut OptOut { get; set; } = new();

    public AfterHours? AfterHours { get; set; }

    public string FromEmail { get; set; }

    public Dictionary<string, bool> Agents { get; set; } = new();
}
