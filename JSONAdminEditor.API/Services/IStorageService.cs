using JSONAdminEditor.Models;

namespace JSONAdminEditor.Services
{
    public interface IStorageService
    {
        Task<(bool Success, string Message)> UploadFileAsync(FileUploadViewModel uploadModel);
        Task<(bool Success, string Message)> OverwriteFileAsync(FileUploadViewModel uploadModel);
        Task<List<ManagedFile>> GetManagedFilesAsync();
        List<ManagedFile> GetManagedFiles();
        bool DeleteFile(string filePath);
        string GetDataFolderPath();
        Task<Customer?> GetCustomerByIdAsync(string customerId);
        Task<List<CustomerLookupResult>> SearchCustomersAsync(string searchTerm);
        Task<bool> ValidateCustomerExistsAsync(string customerName);
        Task<Dictionary<string, object>?> GetCustomerDataAsync(string customerId);
        Task<bool> SaveCustomerDataAsync(string customerId, Dictionary<string, object> customerData);
    }
}
