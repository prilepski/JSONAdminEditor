using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Models;
using Microsoft.Extensions.Options;

namespace JSONAdminEditor.Services
{
    public class StorageServiceFactory : IStorageServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly StorageSettings _storageSettings;

        public StorageServiceFactory(IServiceProvider serviceProvider, IOptions<StorageSettings> storageSettings)
        {
            _serviceProvider = serviceProvider;
            _storageSettings = storageSettings.Value;
        }

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
}
