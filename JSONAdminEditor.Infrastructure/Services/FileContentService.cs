using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Infrastructure.Models;
using Microsoft.Extensions.Options;

namespace JSONAdminEditor.Infrastructure.Services;

public class FileContentService(
    IStorageServiceFactory factory,
    IOptions<StorageSettings> storageSettings) : IFileContentService
{
    private readonly StorageSettings _storageSettings = storageSettings.Value;
    private readonly IStorageServiceFactory _factory = factory;

    public async Task<string> ReadFileAsync(string filePath)
    {
        try
        {
            var storageService = _factory.CreateStorageService(_storageSettings.StorageType);
            string content = await storageService.ReadFileAsync(filePath);
            return content ?? "";
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
            var storageService = _factory.CreateStorageService(_storageSettings.StorageType);
            await storageService.WriteFileAsync(filePath, content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {

        var storageService = _factory.CreateStorageService(_storageSettings.StorageType);
        return await storageService.FileExistsAsync(filePath);
    }
}