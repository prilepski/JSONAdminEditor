using JSONAdminEditor.Models;
using Microsoft.Extensions.Options;

namespace JSONAdminEditor.Services
{
    public interface IFileContentService
    {
        Task<string> ReadFileAsync(string filePath);
        Task<bool> WriteFileAsync(string filePath, string content);
        Task<bool> FileExistsAsync(string filePath);
        string MapToStoragePath(string filePath);
    }

    public class FileContentService : IFileContentService
    {
        private readonly IStorageServiceFactory _storageServiceFactory;
        private readonly StorageSettings _storageSettings;
        private readonly IWebHostEnvironment _environment;

        public FileContentService(IStorageServiceFactory storageServiceFactory, IOptions<StorageSettings> storageSettings, IWebHostEnvironment environment)
        {
            _storageServiceFactory = storageServiceFactory;
            _storageSettings = storageSettings.Value;
            _environment = environment;
        }

        public async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                if (_storageSettings.StorageType.ToLower() == "s3")
                {
                    var s3Service = _storageServiceFactory.CreateStorageService() as S3StorageService;
                    if (s3Service != null)
                    {
                        var key = MapFilePathToS3Key(filePath);
                        try
                        {
                            var content = await s3Service.GetFileContentAsync(key);
                            return content ?? "";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ERROR in FileContentService.ReadFileAsync: {ex.GetType().Name}: {ex.Message}");
                            Console.WriteLine($"Stack trace: {ex.StackTrace}");
                            return "";
                        }
                    }
                    return "";
                }
                else
                {
                    // File system - map relative paths to absolute paths
                    var absolutePath = MapToFileSystemPath(filePath);
                    if (File.Exists(absolutePath))
                    {
                        return await File.ReadAllTextAsync(absolutePath);
                    }
                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OUTER ERROR in FileContentService.ReadFileAsync: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return "";
            }
        }

        public async Task<bool> WriteFileAsync(string filePath, string content)
        {
            if (_storageSettings.StorageType.ToLower() == "s3")
            {
                var s3Service = _storageServiceFactory.CreateStorageService() as S3StorageService;
                if (s3Service != null)
                {
                    var key = MapFilePathToS3Key(filePath);
                    await s3Service.UploadTextToS3Async(content, key);
                    return true;
                }
                return false;
            }
            else
            {
                // File system - map relative paths to absolute paths
                var absolutePath = MapToFileSystemPath(filePath);
                
                // Ensure directory exists
                var directory = Path.GetDirectoryName(absolutePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                await File.WriteAllTextAsync(absolutePath, content);
                return true;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            if (_storageSettings.StorageType.ToLower() == "s3")
            {
                var s3Service = _storageServiceFactory.CreateStorageService() as S3StorageService;
                if (s3Service != null)
                {
                    var key = MapFilePathToS3Key(filePath);
                    return await s3Service.FileExistsAsync(key);
                }
                return false;
            }
            else
            {
                // File system - map relative paths to absolute paths
                var absolutePath = MapToFileSystemPath(filePath);
                return File.Exists(absolutePath);
            }
        }

        public string MapToStoragePath(string filePath)
        {
            if (_storageSettings.StorageType.ToLower() == "s3")
            {
                return MapFilePathToS3Key(filePath);
            }
            return MapToFileSystemPath(filePath);
        }

        private string MapToFileSystemPath(string filePath)
        {
            // If it's already an absolute path, return as-is
            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }
            
            // Map relative paths to wwwroot
            return Path.Combine(_environment.WebRootPath, filePath);
        }

        private string MapFilePathToS3Key(string filePath)
        {
            // Convert Windows/Unix file paths to S3 keys
            // Remove drive letters and leading slashes, replace backslashes with forward slashes
            var key = filePath.Replace('\\', '/');
            
            // Remove common path prefixes to get relative paths
            if (key.Contains("/wwwroot/data/"))
            {
                var index = key.IndexOf("/wwwroot/data/") + "/wwwroot/data/".Length;
                key = key.Substring(index);
            }
            else if (key.Contains("wwwroot/data/"))
            {
                var index = key.IndexOf("wwwroot/data/") + "wwwroot/data/".Length;
                key = key.Substring(index);
            }
            else if (key.StartsWith("data/"))
            {
                // Already relative to data folder
                key = key.Substring("data/".Length);
            }
            else if (key.StartsWith("/data/"))
            {
                key = key.Substring("/data/".Length);
            }
            
            // Remove leading slashes
            key = key.TrimStart('/');
            
            return key;
        }
    }
}
