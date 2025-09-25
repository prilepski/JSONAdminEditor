using JSONAdminEditor.Models;

namespace JSONAdminEditor.Services;

public interface IJsonFileService
{
    Task<T?> LoadJsonFileAsync<T>(string filePath) where T : class, new();
    Task<bool> SaveJsonFileAsync<T>(string filePath, T data);
    List<string> GetUploadedFiles();
    string GetUploadsFolderPath();
}
