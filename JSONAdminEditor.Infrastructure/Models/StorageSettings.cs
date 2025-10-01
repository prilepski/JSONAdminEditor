using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Infrastructure.Models;

public class StorageSettings
{
    public StorageType StorageType { get; set; } = StorageType.FileSystem;
    public S3Settings S3Settings { get; set; } = new();
}


