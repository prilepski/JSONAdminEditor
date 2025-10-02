using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Application.Interfaces;

public interface IStorageServiceFactory
{
    IStorageService CreateStorageService(StorageType type);
}
