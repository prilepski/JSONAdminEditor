using JSONAdminEditor.Application.Interfaces;
using System.Text.Json;

namespace JSONAdminEditor.Services;

public class JsonFileService : IJsonFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IFileContentService _fileContentService;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _uploadsFolder;

    public JsonFileService(
        IWebHostEnvironment environment,
        IFileContentService fileContentService)
    {
        _environment = environment;
        _fileContentService = fileContentService;
        _uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };

        // Ensure uploads directory exists (always local for temporary files)
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }

    public async Task<bool> SaveJsonFileAsync<T>(string filePath, T data)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(data, _jsonOptions);
            return await _fileContentService.WriteFileAsync(filePath, jsonString);
        }
        catch
        {
            return false;
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
        catch
        {
            return new T();
        }
    }

    public List<string> GetUploadedFiles()
    {
        if (!Directory.Exists(_uploadsFolder))
            return new List<string>();

        return Directory.GetFiles(_uploadsFolder, "*.json")
                       .Select(Path.GetFileName)
                       .Where(name => name != null)
                       .Cast<string>()
                       .ToList();
    }

    public string GetUploadsFolderPath() => _uploadsFolder;

}
