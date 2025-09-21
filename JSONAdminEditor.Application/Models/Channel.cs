using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum  Channel
{
    Email = 1,
    Sms = 2,
    Voice = 3
}
