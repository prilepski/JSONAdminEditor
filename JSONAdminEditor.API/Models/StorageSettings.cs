using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Models;

public class StorageSettings
{
    public string StorageType { get; set; } = "FileSystem"; // "FileSystem" or "S3"
    public S3Settings S3Settings { get; set; } = new();
}

public class S3Settings
{
    public string Region { get; set; } = "";
    public string BucketName { get; set; } = "";
    public Dictionary<FileType, string> Properties { get; set; }
}
