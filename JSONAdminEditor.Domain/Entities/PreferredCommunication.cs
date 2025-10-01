using JSONAdminEditor.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Domain.Entities;

public class PreferredCommunication
{
    [Required]
    public Channel Channel { get; set; }

    [Required]
    public int Priority { get; set; }
}
