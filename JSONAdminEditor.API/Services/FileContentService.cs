using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Models;
using Microsoft.Extensions.Options;

namespace JSONAdminEditor.Services
{
    public class FileContentService(
        IStorageServiceFactory storageServiceFactory, 
        IOptions<StorageSettings> storageSettings, 
        IWebHostEnvironment environment) : IFileContentService
    {
        private readonly IStorageServiceFactory _storageServiceFactory = storageServiceFactory;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly bool _isS3Storage = storageSettings.Value.StorageType.Equals("s3", StringComparison.OrdinalIgnoreCase);

        public async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                if (_isS3Storage)
                {
                    if (_storageServiceFactory.CreateStorageService() is S3StorageService s3Service)
                    {
                        var content = await s3Service.GetFileContentAsync(MapFilePathToS3Key(filePath));
                        return content ?? "";
                    }
                    return "";
                }

                var absolutePath = MapToFileSystemPath(filePath);
                return File.Exists(absolutePath) ? await File.ReadAllTextAsync(absolutePath) : "";
            }
            catch
            {
                return "";
            }
        }

        public async Task<bool> WriteFileAsync(string filePath, string content)
        {
            try
            {
                if (_isS3Storage)
                {
                    if (_storageServiceFactory.CreateStorageService() is S3StorageService s3Service)
                    {
                        await s3Service.UploadTextToS3Async(content, MapFilePathToS3Key(filePath));
                        return true;
                    }
                    return false;
                }

                var absolutePath = MapToFileSystemPath(filePath);
                var directory = Path.GetDirectoryName(absolutePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(absolutePath, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            if (_isS3Storage)
            {
                if (_storageServiceFactory.CreateStorageService() is S3StorageService s3Service)
                {
                    return await s3Service.FileExistsAsync(MapFilePathToS3Key(filePath));
                }
                return false;
            }

            return File.Exists(MapToFileSystemPath(filePath));
        }

        public string MapToStoragePath(string filePath) =>
            _isS3Storage ? MapFilePathToS3Key(filePath) : MapToFileSystemPath(filePath);

        private string MapToFileSystemPath(string filePath) =>
            Path.IsPathRooted(filePath) ? filePath : Path.Combine(_environment.WebRootPath, filePath);

        private static string MapFilePathToS3Key(string filePath)
        {
            var key = filePath.Replace('\\', '/');

            var result = key switch
            {
                var k when k.Contains("/wwwroot/data/") => k.Substring(k.IndexOf("/wwwroot/data/") + 14),
                var k when k.Contains("wwwroot/data/") => k.Substring(k.IndexOf("wwwroot/data/") + 13),
                var k when k.StartsWith("data/") => k.Substring(5),
                var k when k.StartsWith("/data/") => k.Substring(6),
                _ => key
            };

            return result.TrimStart('/');
        }
    }
}
