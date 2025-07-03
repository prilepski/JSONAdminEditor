using JSONAdminEditor.Models;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace JSONAdminEditor.Services
{
    public class FileManagementService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _dataFolder;
        private readonly string _customerFolder;

        public FileManagementService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _dataFolder = Path.Combine(_environment.WebRootPath, "data");
            _customerFolder = Path.Combine(_dataFolder, "customers");
            
            // Ensure directories exist
            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }
            if (!Directory.Exists(_customerFolder))
            {
                Directory.CreateDirectory(_customerFolder);
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

                string fileName;
                string filePath;

                switch (uploadModel.FileType)
                {
                    case FileType.Templates:
                        fileName = "templates.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.Customers:
                        fileName = "customers.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.EventTriggers:
                        fileName = "event-triggers.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.EventChannels:
                        fileName = "event-channels.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.OrderTypes:
                        fileName = "order-types.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.CustomerSettings:
                        if (string.IsNullOrWhiteSpace(uploadModel.CustomerName))
                        {
                            return (false, "Customer name is required for Customer Settings files.");
                        }
                        
                        // Use CustomerIdForFilename if available, otherwise fall back to CustomerName
                        var customerIdForFile = !string.IsNullOrWhiteSpace(uploadModel.CustomerIdForFilename) 
                            ? uploadModel.CustomerIdForFilename 
                            : uploadModel.CustomerName;
                        
                        // Sanitize customer ID for filename
                        var sanitizedName = SanitizeFileName(customerIdForFile);
                        fileName = $"{sanitizedName}.json";
                        filePath = Path.Combine(_customerFolder, fileName);
                        break;
                    default:
                        return (false, "Invalid file type selected.");
                }

                // Check if file already exists
                if (File.Exists(filePath))
                {
                    return (false, $"File already exists: {GetDisplayPath(filePath)}. Please confirm if you want to overwrite it.");
                }

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadModel.JsonFile!.CopyToAsync(stream);
                }

                return (true, $"File uploaded successfully: {GetDisplayPath(filePath)}");
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

                string fileName;
                string filePath;

                switch (uploadModel.FileType)
                {
                    case FileType.Templates:
                        fileName = "templates.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.Customers:
                        fileName = "customers.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.EventTriggers:
                        fileName = "event-triggers.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.EventChannels:
                        fileName = "event-channels.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.OrderTypes:
                        fileName = "order-types.json";
                        filePath = Path.Combine(_dataFolder, fileName);
                        break;
                    case FileType.CustomerSettings:
                        if (string.IsNullOrWhiteSpace(uploadModel.CustomerName))
                        {
                            return (false, "Customer name is required for Customer Settings files.");
                        }
                        
                        // Use CustomerIdForFilename if available, otherwise fall back to CustomerName
                        var customerIdForFile = !string.IsNullOrWhiteSpace(uploadModel.CustomerIdForFilename) 
                            ? uploadModel.CustomerIdForFilename 
                            : uploadModel.CustomerName;
                        
                        var sanitizedName = SanitizeFileName(customerIdForFile);
                        fileName = $"{sanitizedName}.json";
                        filePath = Path.Combine(_customerFolder, fileName);
                        break;
                    default:
                        return (false, "Invalid file type selected.");
                }

                // Overwrite the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadModel.JsonFile!.CopyToAsync(stream);
                }

                return (true, $"File overwritten successfully: {GetDisplayPath(filePath)}");
            }
            catch (Exception ex)
            {
                return (false, $"Error overwriting file: {ex.Message}");
            }
        }

        public async Task<List<ManagedFile>> GetManagedFilesAsync()
        {
            var files = new List<ManagedFile>();

            // Add main files
            AddFileIfExists(files, Path.Combine(_dataFolder, "templates.json"), FileType.Templates);
            AddFileIfExists(files, Path.Combine(_dataFolder, "customers.json"), FileType.Customers);
            AddFileIfExists(files, Path.Combine(_dataFolder, "event-triggers.json"), FileType.EventTriggers);
            AddFileIfExists(files, Path.Combine(_dataFolder, "event-channels.json"), FileType.EventChannels);
            AddFileIfExists(files, Path.Combine(_dataFolder, "order-types.json"), FileType.OrderTypes);

            // Add customer files
            if (Directory.Exists(_customerFolder))
            {
                var customerFiles = Directory.GetFiles(_customerFolder, "*.json");
                foreach (var file in customerFiles)
                {
                    var customerId = Path.GetFileNameWithoutExtension(file);
                    
                    // Look up customer information
                    var customer = await GetCustomerByIdAsync(customerId);
                    var displayName = customer != null 
                        ? $"{customer.CompanyName} ({customer.CustomerId})" 
                        : customerId;
                    
                    files.Add(new ManagedFile
                    {
                        FileName = Path.GetFileName(file),
                        FilePath = file,
                        DisplayPath = $"customers/{Path.GetFileName(file)}",
                        FileType = FileType.CustomerSettings,
                        FileTypeDisplay = "Customer Settings",
                        CustomerName = displayName,
                        CanDelete = true,
                        LastModified = File.GetLastWriteTime(file)
                    });
                }
            }

            return files.OrderBy(f => f.FileType).ThenBy(f => f.CustomerName).ToList();
        }

        // Keep the synchronous version for backward compatibility, but without customer lookup
        public List<ManagedFile> GetManagedFiles()
        {
            var files = new List<ManagedFile>();

            // Add main files
            AddFileIfExists(files, Path.Combine(_dataFolder, "templates.json"), FileType.Templates);
            AddFileIfExists(files, Path.Combine(_dataFolder, "customers.json"), FileType.Customers);
            AddFileIfExists(files, Path.Combine(_dataFolder, "event-triggers.json"), FileType.EventTriggers);
            AddFileIfExists(files, Path.Combine(_dataFolder, "event-channels.json"), FileType.EventChannels);
            AddFileIfExists(files, Path.Combine(_dataFolder, "order-types.json"), FileType.OrderTypes);

            // Add customer files (without lookup for performance)
            if (Directory.Exists(_customerFolder))
            {
                var customerFiles = Directory.GetFiles(_customerFolder, "*.json");
                foreach (var file in customerFiles)
                {
                    var customerId = Path.GetFileNameWithoutExtension(file);
                    
                    files.Add(new ManagedFile
                    {
                        FileName = Path.GetFileName(file),
                        FilePath = file,
                        DisplayPath = $"customers/{Path.GetFileName(file)}",
                        FileType = FileType.CustomerSettings,
                        FileTypeDisplay = "Customer Settings",
                        CustomerName = customerId, // Just use customer ID without lookup
                        CanDelete = true,
                        LastModified = File.GetLastWriteTime(file)
                    });
                }
            }

            return files.OrderBy(f => f.FileType).ThenBy(f => f.CustomerName).ToList();
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // Only allow deletion of customer files
                    if (filePath.StartsWith(_customerFolder))
                    {
                        File.Delete(filePath);
                        return true;
                    }
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

            var customersFilePath = Path.Combine(_dataFolder, "customers.json");
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
            
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                return results;

            var customersFilePath = Path.Combine(_dataFolder, "customers.json");
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

            return results.Take(10).ToList(); // Limit to 10 results
        }

        public async Task<bool> ValidateCustomerExistsAsync(string customerName)
        {
            // Extract customer ID from display format "Company Name (CustomerID)" if needed
            var customerIdToCheck = customerName;
            
            // Check if the input is in the display format "Company Name (CustomerID)"
            var match = System.Text.RegularExpressions.Regex.Match(customerName, @"^.+\s\(([^)]+)\)$");
            if (match.Success)
            {
                customerIdToCheck = match.Groups[1].Value;
            }
            
            var searchResults = await SearchCustomersAsync(customerIdToCheck);
            return searchResults.Any(r => 
                string.Equals(r.CustomerId, customerIdToCheck, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.CompanyName, customerIdToCheck, StringComparison.OrdinalIgnoreCase));
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
                FileType.Templates => "Templates",
                FileType.Customers => "Customers",
                FileType.EventTriggers => "Event Triggers",
                FileType.EventChannels => "Event Channels",
                FileType.OrderTypes => "Order Types",
                FileType.CustomerSettings => "Customer Settings",
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

        private string SanitizeFileName(string fileName)
        {
            // Remove invalid characters and replace with underscores
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            
            // Remove extra spaces and replace with underscores
            sanitized = Regex.Replace(sanitized, @"\s+", "_");
            
            return sanitized.ToUpperInvariant();
        }
    }
}
