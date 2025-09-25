namespace JSONAdminEditor.Services;

using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Models;
using Microsoft.Extensions.Options;

public class StorageServiceFactory(
    IServiceProvider serviceProvider,
    IOptions<StorageSettings> storageSettings) : IStorageServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly StorageSettings _storageSettings = storageSettings.Value;

    public IStorageService CreateStorageService()
    {
        return _storageSettings.StorageType.ToLower() switch
        {
            "s3" => _serviceProvider.GetRequiredService<S3StorageService>(),
            "filesystem" => _serviceProvider.GetRequiredService<FileManagementService>(),
            _ => _serviceProvider.GetRequiredService<FileManagementService>() // Default to FileSystem
        };
    }
}