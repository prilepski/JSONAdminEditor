using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Application.Models.Configuration;

namespace JSONAdminEditor.Services;

public interface IConfigRepository
{
    Task SaveGlobalConfigAsync(NotificationMapping config);
    Task<NotificationMapping> GetGlobalConfigAsync();
    Task SaveCustomerConfigAsync(string customerId, CustomerNotificationMapping customerData);
    Task<CustomerNotificationMapping?> GetCustomerConfigAsync(string customerId);
    Task<List<T>?> GetDictionaryDataAsync<T>(FileType fileType) where T : class, new();
    Task SaveDictionaryDataAsync<T>(FileType fileType, List<T> data);
    Task SaveDictionaryRawDataAsync(FileType fileType, string data);
}
