using JSONAdminEditor.Domain.Entities;
using JSONAdminEditor.Domain.Enums;

namespace JSONAdminEditor.Domain.Interfaces;

/// <summary>
/// actual service
/// </summary>
public interface IConfigService
{
    // default
    Task<NotificationMapping> GetGlobalConfigAsync();
    Task SaveGlobalConfigAsync(NotificationMapping config);

    // customer specific
    Task<CustomerNotificationMapping?> GetCustomerConfigAsync(string customerId);
    Task SaveCustomerConfigAsync(string customerId, CustomerNotificationMapping customerData);

    // dictionaries
    Task<List<T>?> GetDictionaryDataAsync<T>(FileType fileType) where T : class, new();
    Task SaveDictionaryDataAsync<T>(FileType fileType, List<T> data);
    Task SaveDictionaryRawDataAsync(FileType fileType, string data);
}
