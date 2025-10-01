using JSONAdminEditor.Domain.Enums;
using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models.Dictionaries;

public class EventChannel
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Channel ChannelName { get; set; }

    public bool IsActive { get; set; }
}
