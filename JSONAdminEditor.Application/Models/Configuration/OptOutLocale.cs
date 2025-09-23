using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models.Configuration;

public class OptOutLocale
{
    [Required]
    [JsonPropertyName("optOutKeywords")]
    public string OptOutKeywords { get; set; } = string.Empty;

    [JsonIgnore]
    public List<string> OptOutKeywordList =>
        [.. OptOutKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

    [Required]
    [JsonPropertyName("optInKeywords")]
    public string OptInKeywords { get; set; } = string.Empty;

    [JsonIgnore]
    public List<string> OptInKeywordList =>
        [.. OptInKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

    [Required]
    [JsonPropertyName("helpKeywords")]
    public string HelpKeywords { get; set; } = string.Empty;

    [JsonIgnore]
    public List<string> HelpKeywordList =>
        [.. HelpKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

    [Required]
    [JsonPropertyName("optOutFooter")]
    public string OptOutFooter { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("optOutPhrase")]
    public string OptOutPhrase { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("optInPhrase")]
    public string OptInPhrase { get; set; } = string.Empty;
    [Required]
    [JsonPropertyName("optInMessage")]
    public string OptInMessage { get; set; } = string.Empty;
    [Required]
    [JsonPropertyName("helpPhrase")]
    public string HelpPhrase { get; set; } = string.Empty;
}
