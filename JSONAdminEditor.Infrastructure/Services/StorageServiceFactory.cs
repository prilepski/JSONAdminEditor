using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Application.Models;
using Microsoft.Extensions.DependencyInjection;

namespace JSONAdminEditor.Infrastructure.Services;

public class StorageServiceFactory(IServiceProvider serviceProvider) : IStorageServiceFactory
{

    public IStorageService CreateStorageService(StorageType type)
    {
        return type switch
        {
            StorageType.S3 => serviceProvider.GetRequiredService<S3StorageService>(),
            StorageType.FileSystem => serviceProvider.GetRequiredService<FileManagementService>(),
            _ => serviceProvider.GetRequiredService<FileManagementService>() // Default to FileSystem
        };
    }
}