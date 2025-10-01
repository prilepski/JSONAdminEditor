using System.Text.Json.Serialization;

namespace JSONAdminEditor.Application.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StorageType
{
    FileSystem,
    S3
}
