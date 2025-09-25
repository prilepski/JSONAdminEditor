using JSONAdminEditor.Models;

namespace JSONAdminEditor.Services;

public interface IJsonFileService
{
    //Task<JsonFileViewModel> ProcessJsonFileAsync(IFormFile file);
    Task<T> LoadJsonFileAsync<T>(string filePath);
    Task<bool> SaveJsonFileAsync<T>(string filePath, T data);
    List<string> GetUploadedFiles();
    string GetUploadsFolderPath();
}
