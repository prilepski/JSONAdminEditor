using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Application.Interfaces;

/// <summary>
/// actual service
/// </summary>
public interface IStorageService
{
    Task<(bool Success, string Message)> UploadFileAsync(FileUploadViewModel uploadModel);
    Task<(bool Success, string Message)> OverwriteFileAsync(FileUploadViewModel uploadModel);

    ////////////////// new implementation
    Task<string?> ReadFileAsync(string filePath);
    Task WriteFileAsync(string filePath, string content);
    Task<bool> FileExistsAsync(string filePath);
}