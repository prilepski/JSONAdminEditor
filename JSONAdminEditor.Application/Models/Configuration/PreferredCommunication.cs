using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Application.Models.Configuration;

public class PreferredCommunication
{
    [Required]
    public Channel Channel { get; set; }

    [Required]
    public int Priority { get; set; }
}
