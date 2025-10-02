using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Domain.Enums;
using Microsoft.Extensions.Hosting;

namespace JSONAdminEditor.Infrastructure.Services;

public class FileManagementService : IStorageService
{
    private readonly string _storagePath;
    private readonly string _dataFolder;
    private readonly string _customerFolder;

    public FileManagementService(IHostEnvironment environment)
    {
        _storagePath = Path.Combine(environment.ContentRootPath, "wwwroot");
        _dataFolder = Path.Combine(_storagePath, "data");
        _customerFolder = Path.Combine(_dataFolder, "customers");

        InitRepository(_storagePath);
        InitRepository(_dataFolder);
        InitRepository(_customerFolder);
    }

    public async Task<(bool Success, string Message)> UploadFileAsync(FileUploadViewModel uploadModel)
    {
        return await ProcessFileAsync(uploadModel, false);
    }

    public async Task<(bool Success, string Message)> OverwriteFileAsync(FileUploadViewModel uploadModel)
    {
        return await ProcessFileAsync(uploadModel, true);
    }

    private static void InitRepository(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private async Task<(bool Success, string Message)> ProcessFileAsync(FileUploadViewModel uploadModel, bool overwrite)
    {
        try
        {
            if (uploadModel.FileType == FileType.None)
                return (false, "Please select a valid file type.");

            var filePath = GetFilePathForType(uploadModel);
            if (filePath.Success == false)
                return (false, filePath.Message);

            if (!overwrite && File.Exists(filePath.Path))
                return (false, $"File already exists: {GetDisplayPath(filePath.Path)}. Please confirm if you want to overwrite it.");

            using var stream = new FileStream(filePath.Path, FileMode.Create);
            await uploadModel.JsonFile!.CopyToAsync(stream);

            var action = overwrite ? "overwritten" : "uploaded";
            return (true, $"File {action} successfully: {GetDisplayPath(filePath.Path)}");
        }
        catch (Exception ex)
        {
            return (false, $"Error processing file: {ex.Message}");
        }
    }

    private (bool Success, string Path, string Message) GetFilePathForType(FileUploadViewModel uploadModel)
    {
        var fileName = uploadModel.FileType switch
        {
            FileType.DictionaryTemplates => "templates.json",
            FileType.DictionaryCustomers => "customers.json",
            FileType.DictionaryEventTriggers => "event-triggers.json",
            FileType.DictionaryEventChannels => "event-channels.json",
            FileType.DictionaryOrderTypes => "order-types.json",
            //FileType.CustomerSettings when !string.IsNullOrWhiteSpace(uploadModel.CustomerName) =>
            //    $"{SanitizeFileName(!string.IsNullOrWhiteSpace(uploadModel.CustomerIdForFilename) ? uploadModel.CustomerIdForFilename : uploadModel.CustomerName)}.json",
            //FileType.CustomerSettings => null,
            _ => null
        };

        if (fileName == null)
            return
                //uploadModel.FileType == FileType.CustomerSettings
                //? (false, "", "Customer name is required for Customer Override files.")
                //:
                (false, "", "Invalid file type selected.");

        var path =
            //uploadModel.FileType == FileType.CustomerSettings
            //? Path.Combine(_customerFolder, fileName)
            //:
            Path.Combine(_dataFolder, "dictionaries", fileName);

        return (true, path, "");
    }

    private string GetDisplayPath(string filePath)
    {
        if (filePath.StartsWith(_customerFolder))
        {
            return $"customers/{Path.GetFileName(filePath)}";
        }
        return Path.GetFileName(filePath);
    }

    ////////////// new implamantation
    public async Task<string?> ReadFileAsync(string filePath)
    {
        string absolutePath = MapToFileSystemPath(filePath);
        return File.Exists(absolutePath) ? await File.ReadAllTextAsync(absolutePath) : null;
    }

    public async Task WriteFileAsync(string filePath, string content)
    {
        var absolutePath = MapToFileSystemPath(filePath);
        var directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(absolutePath, content);
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        return File.Exists(MapToFileSystemPath(filePath));
    }

    private string MapToFileSystemPath(string filePath) =>
        Path.IsPathRooted(filePath)
        ? filePath
        : Path.Combine(_storagePath, filePath);
}
