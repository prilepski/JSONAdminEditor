using JSONAdminEditor.Domain.Enums;

namespace JSONAdminEditor.Infrastructure.Models;

public class S3Settings
{
    public string Region { get; set; } = "";
    public string BucketName { get; set; } = "";
    public Dictionary<FileType, string> Properties { get; set; }
}
