using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Application.Interfaces;

public interface IStorageService
{
    Task<(bool Success, string Message)> UploadFileAsync(FileUploadViewModel uploadModel);
    Task<(bool Success, string Message)> OverwriteFileAsync(FileUploadViewModel uploadModel);
    //Task<List<ManagedFile>> GetManagedFilesAsync();
    //List<ManagedFile> GetManagedFiles();
    bool DeleteFile(string filePath);
    Task<string?> ReadFileAsync(string filePath);
    Task WriteFileAsync(string filePath, string content);
    Task<bool> FileExistsAsync(string filePath);
}