namespace JSONAdminEditor.Services;

using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.API.Exceptions;
using System.Text.Json;

public class JsonFileService : IJsonFileService
{
    private readonly IFileContentService _fileContentService;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonFileService(IFileContentService fileContentService)
    {
        _fileContentService = fileContentService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task<bool> SaveJsonFileAsync<T>(string filePath, T data)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(data, _jsonOptions);
            return await _fileContentService.WriteFileAsync(filePath, jsonString);
        }
        catch (Exception ex)
        {
            throw new JsonFileException($"Failed to save JSON file: {filePath}", ex);
        }
    }

    public async Task<T?> LoadJsonFileAsync<T>(string filePath) where T : class, new()
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
}