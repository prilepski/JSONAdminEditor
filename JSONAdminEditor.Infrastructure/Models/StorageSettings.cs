namespace JSONAdminEditor.Infrastructure.Models;

public class StorageSettings
{
    public string StorageType { get; set; } = "FileSystem"; // "FileSystem" or "S3"
    public S3Settings S3Settings { get; set; } = new();
}


