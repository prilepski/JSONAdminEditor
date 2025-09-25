namespace JSONAdminEditor.Application.Interfaces;

public interface IFileContentService
{
    Task<string> ReadFileAsync(string filePath);
    Task<bool> WriteFileAsync(string filePath, string content);
    Task<bool> FileExistsAsync(string filePath);
    string MapToStoragePath(string filePath);
}