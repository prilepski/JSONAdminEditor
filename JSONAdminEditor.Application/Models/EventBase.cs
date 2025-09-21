using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Application.Models;

public class EventBase
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }
}
