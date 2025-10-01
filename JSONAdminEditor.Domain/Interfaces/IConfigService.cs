using JSONAdminEditor.Domain.Entities;
using JSONAdminEditor.Domain.Enums;

namespace JSONAdminEditor.Domain.Interfaces;

public interface IConfigService
{
    Task SaveGlobalConfigAsync(NotificationMapping config);
    Task<NotificationMapping> GetGlobalConfigAsync();
    Task SaveCustomerConfigAsync(string customerId, CustomerNotificationMapping customerData);
    Task<CustomerNotificationMapping?> GetCustomerConfigAsync(string customerId);
    Task<List<T>?> GetDictionaryDataAsync<T>(FileType fileType) where T : class, new();
    Task SaveDictionaryDataAsync<T>(FileType fileType, List<T> data);
    Task SaveDictionaryRawDataAsync(FileType fileType, string data);
}
