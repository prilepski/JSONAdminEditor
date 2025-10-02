using JSONAdminEditor.Application.Exceptions;
using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Domain.Entities;
using JSONAdminEditor.Domain.Enums;
using JSONAdminEditor.Domain.Interfaces;
using System.Text.Json;

namespace JSONAdminEditor.Application.Services;

/// <summary>
/// actual service
/// </summary>
public class ConfigService : IConfigService
{
    private readonly IFileContentService _fileContentService;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigService(IFileContentService fileContentService)
    {
        _fileContentService = fileContentService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task SaveGlobalConfigAsync(NotificationMapping config)
    {
        string filePath = GetFilePathForType(FileType.ConfigurationDefault);
        await SaveJsonFileAsync(filePath, config);
    }

    public async Task<NotificationMapping> GetGlobalConfigAsync()
    {
        string filePath = GetFilePathForType(FileType.ConfigurationDefault);
        return await LoadJsonFileAsync<NotificationMapping>(filePath);
    }

    public async Task SaveCustomerConfigAsync(string customerId, CustomerNotificationMapping customerData)
    {
        string filePath = GetFilePathForType(FileType.ConfigurationCustomer, customerId);
        await SaveJsonFileAsync(filePath, customerData);
    }

    public async Task<CustomerNotificationMapping?> GetCustomerConfigAsync(string customerId)
    {
        string filePath = GetFilePathForType(FileType.ConfigurationCustomer, customerId);
        return await LoadJsonFileAsync<CustomerNotificationMapping>(filePath);
    }

    public async Task<List<T>?> GetDictionaryDataAsync<T>(FileType fileType) where T : class, new()
    {
        string filePath = GetFilePathForType(fileType);

        return await LoadJsonFileAsync<List<T>>(filePath);
    }

    public async Task SaveDictionaryDataAsync<T>(FileType fileType, List<T> data)
    {
        string filePath = GetFilePathForType(fileType);
        await SaveJsonFileAsync(filePath, data);
    }

    public async Task SaveDictionaryRawDataAsync(FileType fileType, string jsonString)
    {
        string filePath = GetFilePathForType(fileType);
        await _fileContentService.WriteFileAsync(filePath, jsonString);
    }

    private async Task<bool> SaveJsonFileAsync<T>(string filePath, T data)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(data, _jsonOptions);
            await _fileContentService.WriteFileAsync(filePath, jsonString);
            return true;
        }
        catch (Exception ex)
        {
            throw new JsonFileException($"Failed to save JSON file: {filePath}", ex);
        }
    }

    private async Task<T?> LoadJsonFileAsync<T>(string filePath) where T : class, new()
    {
        try
        {
            var jsonContent = await _fileContentService.ReadFileAsync(filePath);
            if (string.IsNullOrEmpty(jsonContent))
                return new T();

            return JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions) ?? new T();
        }
        catch (Exception ex)
        {
            throw new JsonFileException($"Failed to load JSON file: {filePath}", ex);
        }
    }

    private static string GetFilePathForType(FileType fileType, string customerId = null)
    {
        var filePath = fileType switch
        {
            FileType.DictionaryTemplates => "data/dictionaries/templates.json",
            FileType.DictionaryEventTriggers => "data/dictionaries/event-triggers.json",
            FileType.DictionaryEventChannels => "data/dictionaries/event-channels.json",
            FileType.DictionaryOrderTypes => "data/dictionaries/order-types.json",
            FileType.DictionaryCustomers => "data/dictionaries/customers.json",
            FileType.DictionaryLogoUrls => "data/dictionaries/logo-url-mappings.json",
            FileType.ConfigurationDefault => "data/notifications.json",
            FileType.ConfigurationCustomer => null,
            _ => throw new ArgumentException($"Unknown file type: {fileType}")
        };

        if (fileType == FileType.ConfigurationCustomer)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentException("Customer ID is required for customer configuration file.");
            }

            filePath = $"data/customers/{customerId}.json";
        }

        return filePath;
    }
}