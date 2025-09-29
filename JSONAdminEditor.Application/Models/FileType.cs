using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FileType
{
    None,
    ConfigurationDefault,
    ConfigurationCustomer,
    DictionaryTemplates,
    DictionaryCustomers,
    CustomerSett,
    DictionaryEventTriggers,
    DictionaryEventChannels,
    DictionaryOrderTypes,
    DictionaryLogoUrls
}
