namespace JSONAdminEditor.Services;

using JSONAdminEditor.API.Constants;
using JSONAdminEditor.API.Exceptions;
using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Application.Models.Configuration;
using System.Text.Json;

public class ConfigRepository : IConfigRepository
{
    private readonly IFileContentService _fileContentService;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigRepository(IFileContentService fileContentService)
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
        await SaveJsonFileAsync("data/notifications.json", config);
    }

    public async Task<NotificationMapping> GetGlobalConfigAsync()
    {
        return await LoadJsonFileAsync<NotificationMapping>("data/notifications.json");
    }

    public async Task SaveCustomerConfigAsync(string customerId, CustomerNotificationMapping customerData)
    {
        await SaveJsonFileAsync($"data/customers/{customerId}.json", customerData);
    }

    public async Task<CustomerNotificationMapping?> GetCustomerConfigAsync(string customerId)
    {
        return await LoadJsonFileAsync<CustomerNotificationMapping>($"data/customers/{customerId}.json");
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

    private static string GetFilePathForType(FileType fileType) => fileType switch
    {
        FileType.Templates => FilePaths.Templates,
        FileType.EventTriggers => FilePaths.EventTriggers,
        FileType.EventChannels => FilePaths.EventChannels,
        FileType.OrderTypes => FilePaths.OrderTypes,
        FileType.Customers => FilePaths.Customers,
        FileType.LogoUrlMappings => FilePaths.LogoUrlMappings,
        _ => throw new ArgumentException($"Unknown file type: {fileType}")
    };

}