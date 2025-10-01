using System.Text.Json.Serialization;

namespace JSONAdminEditor.Domain.Enums;

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
