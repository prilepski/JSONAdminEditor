using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;
using System.Text.RegularExpressions;
using System.Text.Json;
using JSONAdminEditor.Application.Models.Dictionaries;

namespace JSONAdminEditor.Services;

public class FileManagementService : IStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _dataFolder;
    private readonly string _customerFolder;

    public FileManagementService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _dataFolder = Path.Combine(_environment.WebRootPath, "data");
        _customerFolder = Path.Combine(_dataFolder, "customers");
        
        Directory.CreateDirectory(_dataFolder);
        Directory.CreateDirectory(_customerFolder);
    }

    public async Task<(bool Success, string Message)> UploadFileAsync(FileUploadViewModel uploadModel)
    {
        return await ProcessFileAsync(uploadModel, false);
    }

    public async Task<(bool Success, string Message)> OverwriteFileAsync(FileUploadViewModel uploadModel)
    {
        return await ProcessFileAsync(uploadModel, true);
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
            FileType.Templates => "templates.json",
            FileType.Customers => "customers.json",
            FileType.EventTriggers => "event-triggers.json",
            FileType.EventChannels => "event-channels.json",
            FileType.OrderTypes => "order-types.json",
            FileType.CustomerSettings when !string.IsNullOrWhiteSpace(uploadModel.CustomerName) => 
                $"{SanitizeFileName(!string.IsNullOrWhiteSpace(uploadModel.CustomerIdForFilename) ? uploadModel.CustomerIdForFilename : uploadModel.CustomerName)}.json",
            FileType.CustomerSettings => null,
            _ => null
        };

        if (fileName == null)
            return uploadModel.FileType == FileType.CustomerSettings 
                ? (false, "", "Customer name is required for Customer Override files.")
                : (false, "", "Invalid file type selected.");

        var path = uploadModel.FileType == FileType.CustomerSettings 
            ? Path.Combine(_customerFolder, fileName)
            : Path.Combine(_dataFolder, "dictionaries", fileName);

        return (true, path, "");
    }

    public async Task<List<ManagedFile>> GetManagedFilesAsync()
    {
        var files = GetCoreFiles();
        await AddCustomerFilesAsync(files, true);
        return files.OrderBy(f => f.FileType).ThenBy(f => f.CustomerName).ToList();
    }

    public List<ManagedFile> GetManagedFiles()
    {
        var files = GetCoreFiles();
        AddCustomerFilesAsync(files, false).Wait();
        return files.OrderBy(f => f.FileType).ThenBy(f => f.CustomerName).ToList();
    }

    private List<ManagedFile> GetCoreFiles()
    {
        var files = new List<ManagedFile>();
        var coreFileTypes = new[] 
        {
            ("templates.json", FileType.Templates),
            ("customers.json", FileType.Customers),
            ("event-triggers.json", FileType.EventTriggers),
            ("event-channels.json", FileType.EventChannels),
            ("order-types.json", FileType.OrderTypes)
        };

        foreach (var (fileName, fileType) in coreFileTypes)
        {
            AddFileIfExists(files, Path.Combine(_dataFolder, "dictionaries", fileName), fileType);
        }

        return files;
    }

    private async Task AddCustomerFilesAsync(List<ManagedFile> files, bool lookupCustomerInfo)
    {
        if (!Directory.Exists(_customerFolder)) return;

        var customerFiles = Directory.GetFiles(_customerFolder, "*.json");
        foreach (var file in customerFiles)
        {
            var customerId = Path.GetFileNameWithoutExtension(file);
            var displayName = customerId;

            if (lookupCustomerInfo)
            {
                var customer = await GetCustomerByIdAsync(customerId);
                displayName = customer != null ? $"{customer.CompanyName} ({customer.CustomerId})" : customerId;
            }

            files.Add(new ManagedFile
            {
                FileName = Path.GetFileName(file),
                FilePath = file,
                DisplayPath = $"customers/{Path.GetFileName(file)}",
                FileType = FileType.CustomerSettings,
                FileTypeDisplay = "Customer Override",
                CustomerName = displayName,
                CanDelete = true,
                LastModified = File.GetLastWriteTime(file)
            });
        }
    }

    public bool DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath) && filePath.StartsWith(_customerFolder))
            {
                File.Delete(filePath);
                return true; 
            }
            return false; 
        }
        catch
        {
            return false;
        }
    }

    public string GetDataFolderPath() => _dataFolder;

    public async Task<Customer?> GetCustomerByIdAsync(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return null;

        var customersFilePath = Path.Combine(_dataFolder, "dictionaries", "customers.json");
        if (!File.Exists(customersFilePath))
            return null;

        try
        {
            var jsonContent = await File.ReadAllTextAsync(customersFilePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var customers = System.Text.Json.JsonSerializer.Deserialize<List<Customer>>(jsonContent, options);
            
            if (customers == null) return null;

            return customers.FirstOrDefault(c => 
                string.Equals(c.CustomerId, customerId, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void AddFileIfExists(List<ManagedFile> files, string filePath, FileType fileType)
    {
        if (File.Exists(filePath))
        {
            files.Add(new ManagedFile
            {
                FileName = Path.GetFileName(filePath),
                FilePath = filePath,
                DisplayPath = Path.GetFileName(filePath),
                FileType = fileType,
                FileTypeDisplay = GetFileTypeDisplay(fileType),
                CanDelete = false,
                LastModified = File.GetLastWriteTime(filePath)
            });
        }
    }

    private string GetFileTypeDisplay(FileType fileType)
    {
        return fileType switch
        {
            FileType.None => "None",
            FileType.Templates => "Notification Templates",
            FileType.Customers => "Customers",
            FileType.EventTriggers => "Event Triggers",
            FileType.EventChannels => "Event Channels",
            FileType.OrderTypes => "Order Types",
            FileType.CustomerSettings => "Customer Override",
            _ => fileType.ToString()
        };
    }

    private string GetDisplayPath(string filePath)
    {
        if (filePath.StartsWith(_customerFolder))
        {
            return $"customers/{Path.GetFileName(filePath)}";
        }
        return Path.GetFileName(filePath);
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return Regex.Replace(sanitized, @"\s+", "_").ToUpperInvariant();
    }
}
