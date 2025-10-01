using JSONAdminEditor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Domain.Entities;

public class EventMapping
{
    [Required]
    public string Event { get; set; }

    [Required]
    public string OrderType { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    /// <summary>
    /// Dictionary mapping Channel enum values to template IDs
    /// </summary>
    public Dictionary<Channel, string> Templates { get; set; } = [];

    public bool IsSuppressed { get; set; }

    public List<PreferredCommunication> PreferredCommunication { get; set; } = [];

    public Dictionary<string, string> ContentVariables { get; set; } = [];

    public Dictionary<string, bool> TriggerConditions { get; set; } = [];
}
