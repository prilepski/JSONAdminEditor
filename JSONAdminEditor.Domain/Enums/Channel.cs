using System.Text.Json.Serialization;

namespace JSONAdminEditor.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum  Channel
{
    Email = 1,
    Sms = 2,
    Voice = 3
}
