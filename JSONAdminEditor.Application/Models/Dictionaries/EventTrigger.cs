namespace JSONAdminEditor.Application.Models.Dictionaries;

public class EventTrigger
{
    public string EventName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool ByOrderType { get; set; }
}
