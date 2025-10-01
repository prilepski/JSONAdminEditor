namespace JSONAdminEditor.Application.Interfaces;

/// <summary>
/// actual service
/// </summary>
public interface IFileContentService
{
    Task<string> ReadFileAsync(string filePath);
    Task<bool> WriteFileAsync(string filePath, string content);
}