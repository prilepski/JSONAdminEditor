using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Application.Interfaces;


/// <summary>
/// actual service
/// </summary>
public interface IStorageServiceFactory
{
    IStorageService CreateStorageService(StorageType type);
}