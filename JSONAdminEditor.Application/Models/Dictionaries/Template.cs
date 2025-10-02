using JSONAdminEditor.Domain.Enums;

namespace JSONAdminEditor.Application.Models.Dictionaries;

public class Template
{
    public string TemplateId { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public Channel ChannelType { get; set; }
}
