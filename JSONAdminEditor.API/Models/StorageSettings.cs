namespace JSONAdminEditor.Models
{
    public class StorageSettings
    {
        public string StorageType { get; set; } = "FileSystem"; // "FileSystem" or "S3"
        public S3Settings S3Settings { get; set; } = new();
    }

    public class S3Settings
    {
        public string Region { get; set; } = "";
        public string BucketName { get; set; } = "";
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public bool UseCredentialsFromEnvironment { get; set; } = true;
    }
}
