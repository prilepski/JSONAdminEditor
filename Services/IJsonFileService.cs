using JSONAdminEditor.Models;

namespace JSONAdminEditor.Services
{
    public interface IJsonFileService
    {
        Task<JsonFileViewModel> ProcessJsonFileAsync(IFormFile file);
        Task<JsonFileViewModel> LoadJsonFileAsync(string filePath);
        Task<bool> SaveJsonFileAsync(string filePath, List<Dictionary<string, object>> tableData);
        List<string> GetUploadedFiles();
        string GetUploadsFolderPath();
    }
}
