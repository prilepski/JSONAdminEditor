using JSONAdminEditor.Application.Models.Structure;
using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Application.Models;

public class CustomerEventMapping : EventMapping
{

    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ContentVariablesOverrides { get; set; } = [];
}
