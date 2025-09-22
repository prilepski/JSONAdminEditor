using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models.Dictionaries;

public class LogoUrl
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}