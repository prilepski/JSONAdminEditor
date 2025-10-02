namespace JSONAdminEditor.Application.Interfaces;

public interface IFileContentService
{
    Task<string> ReadFileAsync(string filePath);
    Task<bool> WriteFileAsync(string filePath, string content);
}