using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models.Dictionaries;

public class LogoUrlMapping
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}