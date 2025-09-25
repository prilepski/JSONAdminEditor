using Newtonsoft.Json;
using JSONAdminEditor.Application.Interfaces;

namespace JSONAdminEditor.Services;

public class JsonFileService : IJsonFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IFileContentService _fileContentService;
    private readonly string _uploadsFolder;

    public JsonFileService(
        IWebHostEnvironment environment,
        IFileContentService fileContentService)
    {
        _environment = environment;
        _fileContentService = fileContentService;
        _uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

        // Ensure uploads directory exists (always local for temporary files)
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }

    public async Task<T> LoadJsonFileAsync<T>(string filePath)
    {
        var jsonContent = await _fileContentService.ReadFileAsync(filePath);
        // Try to parse and convert to table format
        return JsonConvert.DeserializeObject<T>(jsonContent);
    }

    public async Task<bool> SaveJsonFileAsync<T>(string filePath, T data)
    {
        try
        {
            var jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            return await _fileContentService.WriteFileAsync(filePath, jsonString);
        }
        catch
        {
            return false;
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
