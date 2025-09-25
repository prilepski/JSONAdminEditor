using Amazon.S3;
using Amazon.S3.Model;
using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;

namespace JSONAdminEditor.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Settings _s3Settings;
    private readonly string _bucketName;

    public S3StorageService(IAmazonS3 s3Client, IOptions<StorageSettings> storageSettings)
    {
        try
        {
            Console.WriteLine("S3StorageService constructor - Starting initialization");
            _s3Client = s3Client;
            _s3Settings = storageSettings.Value.S3Settings;
            _bucketName = _s3Settings.BucketName;
            Console.WriteLine($"S3StorageService constructor - BucketName: '{_bucketName}', AccessKey: '{_s3Settings.AccessKey?.Substring(0, Math.Min(4, _s3Settings.AccessKey.Length))}...'");
            Console.WriteLine("S3StorageService constructor - Completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in S3StorageService constructor: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
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

    public async Task<List<ManagedFile>> GetManagedFilesAsync()
    {
        var files = new List<ManagedFile>();

        try
        {
            // Add main dictionary files
            await AddFileIfExistsAsync(files, "dictionaries/templates.json", FileType.Templates);
            await AddFileIfExistsAsync(files, "dictionaries/customers.json", FileType.Customers);
            await AddFileIfExistsAsync(files, "dictionaries/event-triggers.json", FileType.EventTriggers);
            await AddFileIfExistsAsync(files, "dictionaries/event-channels.json", FileType.EventChannels);
            await AddFileIfExistsAsync(files, "dictionaries/order-types.json", FileType.OrderTypes);

            // Add customer files
            //var customerFiles = await ListCustomerFilesAsync();
            //foreach (var key in customerFiles)
            //{
            //    var customerId = GetCustomerIdFromKey(key);
                
            //    // Look up customer information
            //    var customer = await GetCustomerByIdAsync(customerId);
            //    var displayName = customer != null 
            //        ? $"{customer.CompanyName} ({customer.CustomerId})" 
            //        : customerId;

            //    var lastModified = await GetFileLastModifiedAsync(key);
                
            //    files.Add(new ManagedFile
            //    {
            //        FileName = GetFileNameFromKey(key),
            //        FilePath = key,
            //        DisplayPath = GetDisplayPath(key),
            //        FileType = FileType.CustomerSettings,
            //        FileTypeDisplay = "Customer Override",
            //        CustomerName = displayName,
            //        CanDelete = true,
            //        LastModified = lastModified
            //    });
            //}
        }
        catch (Exception)
        {
            // Return empty list on error
        }

        return files.OrderBy(f => f.FileType).ThenBy(f => f.CustomerName).ToList();
    }

    public List<ManagedFile> GetManagedFiles()
    {
        // For S3, we need to use async methods, so this will be a simplified synchronous version
        // In practice, you should use GetManagedFilesAsync for S3
        return new List<ManagedFile>();
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
                    BucketName = _bucketName,
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

    public string GetDataFolderPath() => $"s3://{_bucketName}/";

    //public async Task<Customer?> GetCustomerByIdAsync(string customerId)
    //{
    //    if (string.IsNullOrWhiteSpace(customerId))
    //        return null;

    //    try
    //    {
    //        var customersJson = await GetFileContentAsync("dictionaries/customers.json");
    //        if (string.IsNullOrEmpty(customersJson))
    //            return null;

    //        var options = new JsonSerializerOptions
    //        {
    //            PropertyNameCaseInsensitive = true
    //        };
            
    //        var customers = JsonSerializer.Deserialize<List<Customer>>(customersJson, options);
            
    //        if (customers == null) return null;

    //        return customers.FirstOrDefault(c => 
    //            string.Equals(c.CustomerId, customerId, StringComparison.OrdinalIgnoreCase));
    //    }
    //    catch (Exception)
    //    {
    //        return null;
    //    }
    //}

    //public async Task<List<CustomerLookupResult>> SearchCustomersAsync(string searchTerm)
    //{
    //    var results = new List<CustomerLookupResult>();
        
    //    if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
    //        return results;

    //    try
    //    {
    //        var customersJson = await GetFileContentAsync("dictionaries/customers.json");
    //        if (string.IsNullOrEmpty(customersJson))
    //            return results;

    //        var options = new JsonSerializerOptions
    //        {
    //            PropertyNameCaseInsensitive = true
    //        };
            
    //        var customers = JsonSerializer.Deserialize<List<Customer>>(customersJson, options);
            
    //        if (customers == null) return results;

    //        // Check if the search term is in the display format "Company Name (Customer ID)"
    //        var companyNameFromDisplay = "";
    //        var customerIdFromDisplay = "";
    //        var match = Regex.Match(searchTerm, @"^(.+)\s\(([^)]+)\)$");
            
    //        if (match.Success)
    //        {
    //            // Extract company name and customer ID from display format
    //            companyNameFromDisplay = match.Groups[1].Value.Trim().ToLowerInvariant();
    //            customerIdFromDisplay = match.Groups[2].Value.Trim().ToLowerInvariant();
    //        }
    //        else
    //        {
    //            // Use the original search term
    //            searchTerm = searchTerm.ToLowerInvariant();
    //        }

    //        foreach (var customer in customers)
    //        {
    //            bool isMatch = false;
    //            string matchType = "";
                
    //            if (match.Success)
    //            {
    //                // If search term is in display format, search for exact matches or partial matches
    //                var customerIdMatch = customer.CustomerId?.ToLowerInvariant().Contains(customerIdFromDisplay) == true;
    //                var companyNameMatch = customer.CompanyName?.ToLowerInvariant().Contains(companyNameFromDisplay) == true;
                    
    //                // Also check if either part matches the customer data
    //                var customerIdMatchesCompany = customer.CustomerId?.ToLowerInvariant().Contains(companyNameFromDisplay) == true;
    //                var companyNameMatchesId = customer.CompanyName?.ToLowerInvariant().Contains(customerIdFromDisplay) == true;
                    
    //                isMatch = customerIdMatch || companyNameMatch || customerIdMatchesCompany || companyNameMatchesId;
    //                matchType = customerIdMatch ? "CustomerId" : "CompanyName";
    //            }
    //            else
    //            {
    //                // Regular search against both fields
    //                var customerIdMatch = customer.CustomerId?.ToLowerInvariant().Contains(searchTerm) == true;
    //                var companyNameMatch = customer.CompanyName?.ToLowerInvariant().Contains(searchTerm) == true;
                    
    //                isMatch = customerIdMatch || companyNameMatch;
    //                matchType = customerIdMatch ? "CustomerId" : "CompanyName";
    //            }
                
    //            if (isMatch)
    //            {
    //                results.Add(new CustomerLookupResult
    //                {
    //                    CustomerId = customer.CustomerId ?? "",
    //                    CompanyName = customer.CompanyName ?? "",
    //                    MatchType = matchType
    //                });
    //            }
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        // Return empty list on error
    //    }

    //    return results.Take(10).ToList(); // Limit to 10 results
    //}

    //public async Task<bool> ValidateCustomerExistsAsync(string customerName)
    //{
    //    if (string.IsNullOrWhiteSpace(customerName))
    //        return false;

    //    // Extract customer ID from display format "Company Name (CustomerID)" if needed
    //    var customerIdToCheck = customerName.Trim();
        
    //    // Check if the input is in the display format "Company Name (CustomerID)"
    //    var match = Regex.Match(customerName, @"^.+\s\(([^)]+)\)$");
    //    if (match.Success)
    //    {
    //        customerIdToCheck = match.Groups[1].Value.Trim();
    //    }
        
    //    // Get the customer directly by ID for exact validation
    //    var customer = await GetCustomerByIdAsync(customerIdToCheck);
    //    if (customer != null)
    //    {
    //        return true;
    //    }
        
    //    // If not found by exact ID match, try searching by company name
    //    try
    //    {
    //        var customersJson = await GetFileContentAsync("dictionaries/customers.json");
    //        if (string.IsNullOrEmpty(customersJson))
    //            return false;

    //        var options = new JsonSerializerOptions
    //        {
    //            PropertyNameCaseInsensitive = true
    //        };
            
    //        var customers = JsonSerializer.Deserialize<List<Customer>>(customersJson, options);
            
    //        if (customers == null) return false;

    //        // Check for exact matches (case-insensitive)
    //        return customers.Any(c => 
    //            string.Equals(c.CustomerId, customerIdToCheck, StringComparison.OrdinalIgnoreCase) ||
    //            string.Equals(c.CompanyName, customerIdToCheck, StringComparison.OrdinalIgnoreCase));
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //}

    //public async Task<Dictionary<string, object>?> GetCustomerDataAsync(string customerId)
    //{
    //    try
    //    {
    //        var sanitizedId = SanitizeFileName(customerId);
    //        var key = $"customers/{sanitizedId}.json";
            
    //        var jsonContent = await GetFileContentAsync(key);
    //        if (string.IsNullOrEmpty(jsonContent))
    //        {
    //            return new Dictionary<string, object>();
    //        }

    //        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
    //        return data ?? new Dictionary<string, object>();
    //    }
    //    catch (Exception)
    //    {
    //        return new Dictionary<string, object>();
    //    }
    //}

    //public async Task<bool> SaveCustomerDataAsync(string customerId, Dictionary<string, object> customerData)
    //{
    //    try
    //    {
    //        var sanitizedId = SanitizeFileName(customerId);
    //        var key = $"customers/{sanitizedId}.json";

    //        var options = new JsonSerializerOptions
    //        {
    //            WriteIndented = true,
    //            PropertyNamingPolicy = null
    //        };

    //        var jsonContent = JsonSerializer.Serialize(customerData, options);
    //        await UploadTextToS3Async(jsonContent, key);
            
    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        return false;
    //    }
    //}

    #region Private Helper Methods

    private string GetS3KeyForFileType(FileUploadViewModel uploadModel)
    {
        return uploadModel.FileType switch
        {
            FileType.Templates => "dictionaries/templates.json",
            FileType.Customers => "dictionaries/customers.json",
            FileType.EventTriggers => "dictionaries/event-triggers.json",
            FileType.EventChannels => "dictionaries/event-channels.json",
            FileType.OrderTypes => "dictionaries/order-types.json",
            FileType.CustomerSettings => GetCustomerFileKey(uploadModel),
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
                BucketName = _bucketName,
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
            BucketName = _bucketName,
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
            BucketName = _bucketName,
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
                BucketName = _bucketName,
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

    private async Task AddFileIfExistsAsync(List<ManagedFile> files, string key, FileType fileType)
    {
        if (await FileExistsAsync(key))
        {
            var lastModified = await GetFileLastModifiedAsync(key);
            
            files.Add(new ManagedFile
            {
                FileName = GetFileNameFromKey(key),
                FilePath = key,
                DisplayPath = GetFileNameFromKey(key),
                FileType = fileType,
                FileTypeDisplay = GetFileTypeDisplay(fileType),
                CanDelete = false,
                LastModified = lastModified
            });
        }
    }

    private async Task<DateTime> GetFileLastModifiedAsync(string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectMetadataAsync(request);
            return response.LastModified ?? DateTime.MinValue;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    private async Task<List<string>> ListCustomerFilesAsync()
    {
        var keys = new List<string>();

        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "customers/",
                Delimiter = "/"
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                keys.AddRange(response.S3Objects.Where(obj => obj.Key.EndsWith(".json")).Select(obj => obj.Key));
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated == true);
        }
        catch
        {
            // Return empty list on error
        }

        return keys;
    }

    private string GetCustomerIdFromKey(string key)
    {
        // Extract customer ID from "customers/CUSTOMERID.json"
        var fileName = GetFileNameFromKey(key);
        return Path.GetFileNameWithoutExtension(fileName);
    }

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
