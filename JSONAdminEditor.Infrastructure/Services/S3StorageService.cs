using Amazon.S3;
using Amazon.S3.Model;
using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Domain.Enums;
using JSONAdminEditor.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;

namespace JSONAdminEditor.Infrastructure.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Settings _s3Settings;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(IAmazonS3 s3Client, 
        IOptions<StorageSettings> storageSettings,
        ILogger<S3StorageService> logger)
    {
        _logger = logger;
        _s3Client = s3Client;

        try
        {
            _s3Settings = storageSettings.Value.S3Settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR in S3StorageService constructor: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<(bool Success, string Message)> UploadFileAsync(FileUploadViewModel uploadModel)
    {
        try
        {
            // Validate file type selection
            if (uploadModel.FileType == FileType.None)
            {
                return (false, "Please select a valid file type.");
            }

            string key = GetS3KeyForFileType(uploadModel);
            if (string.IsNullOrEmpty(key))
            {
                return (false, "Invalid file type selected.");
            }

            // Check if file already exists
            if (await FileExistsAsync(key))
            {
                return (false, $"File already exists: {GetDisplayPath(key)}. Please confirm if you want to overwrite it.");
            }

            // Upload the file
            await UploadFileToS3Async(uploadModel.JsonFile!, key);

            return (true, $"File uploaded successfully: {GetDisplayPath(key)}");
        }
        catch (Exception ex)
        {
            return (false, $"Error uploading file: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> OverwriteFileAsync(FileUploadViewModel uploadModel)
    {
        try
        {
            // Validate file type selection
            if (uploadModel.FileType == FileType.None)
            {
                return (false, "Please select a valid file type.");
            }

            string key = GetS3KeyForFileType(uploadModel);
            if (string.IsNullOrEmpty(key))
            {
                return (false, "Invalid file type selected.");
            }

            // Overwrite the file
            await UploadFileToS3Async(uploadModel.JsonFile!, key);

            return (true, $"File overwritten successfully: {GetDisplayPath(key)}");
        }
        catch (Exception ex)
        {
            return (false, $"Error overwriting file: {ex.Message}");
        }
    }


    public bool DeleteFile(string key)
    {
        try
        {
            // Only allow deletion of customer files
            if (key.StartsWith("customers/"))
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _s3Settings.BucketName,
                    Key = key
                };

                _s3Client.DeleteObjectAsync(deleteRequest).Wait();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }


    public async Task<string?> ReadFileAsync(string filePath)
    {
        //initialy it was GetFileContentAsync
        return await GetFileContentAsync(MapFilePathToS3Key(filePath));
    }

    private static string MapFilePathToS3Key(string filePath)
    {
        var key = filePath.Replace('\\', '/');

        var result = key switch
        {
            var k when k.Contains("/wwwroot/data/") => k.Substring(k.IndexOf("/wwwroot/data/") + 14),
            var k when k.Contains("wwwroot/data/") => k.Substring(k.IndexOf("wwwroot/data/") + 13),
            var k when k.StartsWith("data/") => k.Substring(5),
            var k when k.StartsWith("/data/") => k.Substring(6),
            _ => key
        };

        return result.TrimStart('/');
    }

    public async Task WriteFileAsync(string filePath, string content)
    {
        //initialy if was UploadTextToS3Async
        await UploadTextToS3Async(content, filePath);
    }

    #region Private Helper Methods

    private string GetS3KeyForFileType(FileUploadViewModel uploadModel)
    {
        return uploadModel.FileType switch
        {
            FileType.DictionaryTemplates => "dictionaries/templates.json",
            FileType.DictionaryCustomers => "dictionaries/customers.json",
            FileType.DictionaryEventTriggers => "dictionaries/event-triggers.json",
            FileType.DictionaryEventChannels => "dictionaries/event-channels.json",
            FileType.DictionaryOrderTypes => "dictionaries/order-types.json",
            //FileType.CustomerSettings => GetCustomerFileKey(uploadModel),
            _ => ""
        };
    }

    private string GetCustomerFileKey(FileUploadViewModel uploadModel)
    {
        if (string.IsNullOrWhiteSpace(uploadModel.CustomerName))
            return "";

        var customerIdForFile = !string.IsNullOrWhiteSpace(uploadModel.CustomerIdForFilename) 
            ? uploadModel.CustomerIdForFilename 
            : uploadModel.CustomerName;
        
        var sanitizedName = SanitizeFileName(customerIdForFile);
        return $"customers/{sanitizedName}.json";
    }

    //check if file exists
    public async Task<bool> FileExistsAsync(string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private async Task UploadFileToS3Async(IFormFile file, string key)
    {
        using var stream = file.OpenReadStream();
        
        var request = new PutObjectRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = "application/json"
        };

        await _s3Client.PutObjectAsync(request);
    }

    //upload files
    public async Task UploadTextToS3Async(string content, string key)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        using var stream = new MemoryStream(bytes);
        
        var request = new PutObjectRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = "application/json"
        };

        await _s3Client.PutObjectAsync(request);
    }

    //read files
    public async Task<string> GetFileContentAsync(string key)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            };
            using var response = await _s3Client.GetObjectAsync(request);
            using var reader = new StreamReader(response.ResponseStream);
            return await reader.ReadToEndAsync();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return "";
        }
    }

    //private async Task AddFileIfExistsAsync(List<ManagedFile> files, string key, FileType fileType)
    //{
    //    if (await FileExistsAsync(key))
    //    {
    //        var lastModified = await GetFileLastModifiedAsync(key);
            
    //        files.Add(new ManagedFile
    //        {
    //            FileName = GetFileNameFromKey(key),
    //            FilePath = key,
    //            DisplayPath = GetFileNameFromKey(key),
    //            FileType = fileType,
    //            FileTypeDisplay = GetFileTypeDisplay(fileType),
    //            CanDelete = false,
    //            LastModified = lastModified
    //        });
    //    }
    //}

    //private async Task<DateTime> GetFileLastModifiedAsync(string key)
    //{
    //    try
    //    {
    //        var request = new GetObjectMetadataRequest
    //        {
    //            BucketName = _s3Settings.BucketName,
    //            Key = key
    //        };

    //        var response = await _s3Client.GetObjectMetadataAsync(request);
    //        return response.LastModified ?? DateTime.MinValue;
    //    }
    //    catch
    //    {
    //        return DateTime.MinValue;
    //    }
    //}

    //private async Task<List<string>> ListCustomerFilesAsync()
    //{
    //    var keys = new List<string>();

    //    try
    //    {
    //        var request = new ListObjectsV2Request
    //        {
    //            BucketName = _s3Settings.BucketName,
    //            Prefix = "customers/",
    //            Delimiter = "/"
    //        };

    //        ListObjectsV2Response response;
    //        do
    //        {
    //            response = await _s3Client.ListObjectsV2Async(request);
    //            keys.AddRange(response.S3Objects.Where(obj => obj.Key.EndsWith(".json")).Select(obj => obj.Key));
    //            request.ContinuationToken = response.NextContinuationToken;
    //        } while (response.IsTruncated == true);
    //    }
    //    catch
    //    {
    //        // Return empty list on error
    //    }

    //    return keys;
    //}

    //private string GetCustomerIdFromKey(string key)
    //{
    //    // Extract customer ID from "customers/CUSTOMERID.json"
    //    var fileName = GetFileNameFromKey(key);
    //    return Path.GetFileNameWithoutExtension(fileName);
    //}

    private string GetFileNameFromKey(string key)
    {
        return Path.GetFileName(key);
    }

    private string GetDisplayPath(string key)
    {
        if (key.StartsWith("customers/"))
        {
            return key;
        }
        return GetFileNameFromKey(key);
    }

    //private string GetFileTypeDisplay(FileType fileType)
    //{
    //    return fileType switch
    //    {
    //        FileType.None => "None",
    //        FileType.Templates => "Notification Templates",
    //        FileType.Customers => "Customers",
    //        FileType.EventTriggers => "Event Triggers",
    //        FileType.EventChannels => "Event Channels",
    //        FileType.OrderTypes => "Order Types",
    //        FileType.CustomerSettings => "Customer Override",
    //        _ => fileType.ToString()
    //    };
    //}

    private string SanitizeFileName(string fileName)
    {
        // Remove invalid characters and replace with underscores
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        
        // Remove extra spaces and replace with underscores
        sanitized = Regex.Replace(sanitized, @"\s+", "_");
        
        return sanitized.ToUpperInvariant();
    }

    #endregion
}
