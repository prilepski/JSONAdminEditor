using JSONAdminEditor.Models;
using System.Text.RegularExpressions;
using System.Text.Json;
using JSONAdminEditor.Application.Models.Dictionaries;

namespace JSONAdminEditor.Services
{
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

        public async Task<List<CustomerLookupResult>> SearchCustomersAsync(string searchTerm)
        {
            var results = new List<CustomerLookupResult>();

            var customersFilePath = Path.Combine(_dataFolder, "dictionaries", "customers.json");
            if (!File.Exists(customersFilePath))
                return results;

            try
            {
                var jsonContent = await File.ReadAllTextAsync(customersFilePath);
                
                // Configure JsonSerializer options for case-insensitive property matching
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var customers = System.Text.Json.JsonSerializer.Deserialize<List<Customer>>(jsonContent, options);
                
                if (customers == null) return results;

                // If search term is empty, return all customers
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    foreach (var customer in customers)
                    {
                        results.Add(new CustomerLookupResult
                        {
                            CustomerId = customer.CustomerId ?? "",
                            CompanyName = customer.CompanyName ?? "",
                            MatchType = "All"
                        });
                    }
                    return results;
                }

                // Check if the search term is in the display format "Company Name (Customer ID)"
                var companyNameFromDisplay = "";
                var customerIdFromDisplay = "";
                var match = System.Text.RegularExpressions.Regex.Match(searchTerm, @"^(.+)\s\(([^)]+)\)$");
                
                if (match.Success)
                {
                    // Extract company name and customer ID from display format
                    companyNameFromDisplay = match.Groups[1].Value.Trim().ToLowerInvariant();
                    customerIdFromDisplay = match.Groups[2].Value.Trim().ToLowerInvariant();
                }
                else
                {
                    // Use the original search term
                    searchTerm = searchTerm.ToLowerInvariant();
                }

                foreach (var customer in customers)
                {
                    bool isMatch = false;
                    string matchType = "";
                    
                    if (match.Success)
                    {
                        // If search term is in display format, search for exact matches or partial matches
                        var customerIdMatch = customer.CustomerId?.ToLowerInvariant().Contains(customerIdFromDisplay) == true;
                        var companyNameMatch = customer.CompanyName?.ToLowerInvariant().Contains(companyNameFromDisplay) == true;
                        
                        // Also check if either part matches the customer data
                        var customerIdMatchesCompany = customer.CustomerId?.ToLowerInvariant().Contains(companyNameFromDisplay) == true;
                        var companyNameMatchesId = customer.CompanyName?.ToLowerInvariant().Contains(customerIdFromDisplay) == true;
                        
                        isMatch = customerIdMatch || companyNameMatch || customerIdMatchesCompany || companyNameMatchesId;
                        matchType = customerIdMatch ? "CustomerId" : "CompanyName";
                    }
                    else
                    {
                        // Regular search against both fields
                        var customerIdMatch = customer.CustomerId?.ToLowerInvariant().Contains(searchTerm) == true;
                        var companyNameMatch = customer.CompanyName?.ToLowerInvariant().Contains(searchTerm) == true;
                        
                        isMatch = customerIdMatch || companyNameMatch;
                        matchType = customerIdMatch ? "CustomerId" : "CompanyName";
                    }
                    
                    if (isMatch)
                    {
                        results.Add(new CustomerLookupResult
                        {
                            CustomerId = customer.CustomerId ?? "",
                            CompanyName = customer.CompanyName ?? "",
                            MatchType = matchType
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }

            return string.IsNullOrWhiteSpace(searchTerm) ? results : results.Take(10).ToList();
        }

        public async Task<bool> ValidateCustomerExistsAsync(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                return false;

            // Extract customer ID from display format "Company Name (CustomerID)" if needed
            var customerIdToCheck = customerName.Trim();
            
            // Check if the input is in the display format "Company Name (CustomerID)"
            var match = System.Text.RegularExpressions.Regex.Match(customerName, @"^.+\s\(([^)]+)\)$");
            if (match.Success)
            {
                customerIdToCheck = match.Groups[1].Value.Trim();
            }
            
            // Get the customer directly by ID for exact validation
            var customer = await GetCustomerByIdAsync(customerIdToCheck);
            if (customer != null)
            {
                return true;
            }
            
            // If not found by exact ID match, try searching by company name
            // This handles cases where the user typed a company name directly
            var customersFilePath = Path.Combine(_dataFolder, "dictionaries", "customers.json");
            if (!File.Exists(customersFilePath))
                return false;

            try
            {
                var jsonContent = await File.ReadAllTextAsync(customersFilePath);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var customers = System.Text.Json.JsonSerializer.Deserialize<List<Customer>>(jsonContent, options);
                
                if (customers == null) return false;

                // Check for exact matches (case-insensitive)
                return customers.Any(c => 
                    string.Equals(c.CustomerId, customerIdToCheck, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(c.CompanyName, customerIdToCheck, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Dictionary<string, object>?> GetCustomerDataAsync(string customerId)
        {
            try
            {
                var sanitizedId = SanitizeFileName(customerId);
                var filePath = Path.Combine(_customerFolder, $"{sanitizedId}.json");
                
                if (!File.Exists(filePath))
                {
                    return new Dictionary<string, object>();
                }

                var jsonContent = await File.ReadAllTextAsync(filePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);
                return data ?? new Dictionary<string, object>();
            }
            catch (Exception)
            {
                return new Dictionary<string, object>();
            }
        }

        public async Task<bool> SaveCustomerDataAsync(string customerId, Dictionary<string, object> customerData)
        {
            try
            {
                var sanitizedId = SanitizeFileName(customerId);
                var filePath = Path.Combine(_customerFolder, $"{sanitizedId}.json");
                
                // Ensure directory exists
                Directory.CreateDirectory(_customerFolder);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null
                };

                var jsonContent = JsonSerializer.Serialize(customerData, options);
                await File.WriteAllTextAsync(filePath, jsonContent);
                
                return true;
            }
            catch (Exception)
            {
                return false;
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
}
