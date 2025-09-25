namespace JSONAdminEditor.Application.Models.Configuration;

public class CustomerEventMapping : EventMapping
{
    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ContentVariablesOverrides { get; set; } = [];
}
