using JSONAdminEditor.Application.Models.Structure;

namespace JSONAdminEditor.Application.Models
{
    public class NotificationMessageMapping 
    {
        public OptOut OptOut { get; set; } = new();
        public string FromEmail { get; set; }

        public Dictionary<string, bool> Agents { get; set; } = new();

        public EventMapping? EventMapping { get; set; }
    }
}
