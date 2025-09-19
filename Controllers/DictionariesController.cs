using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using JSONAdminEditor.Security;
using System.Text.Json;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DictionariesController : ControllerBase
{
    private readonly IStorageService _storageService;
    private readonly IJsonFileService _jsonFileService;
    private readonly UniqueFieldValidationService _validationService;

    public DictionariesController(IStorageServiceFactory storageServiceFactory, IJsonFileService jsonFileService, UniqueFieldValidationService validationService)
    {
        _storageService = storageServiceFactory.CreateStorageService();
        _jsonFileService = jsonFileService;
        _validationService = validationService;
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetDictionaryData(int fileType)
    {
        try
        {
            if (Enum.IsDefined(typeof(FileType), fileType))
            {
                var selectedFileType = (FileType)fileType;
                var filePath = GetDictionaryFilePath(selectedFileType);
                
                if (!string.IsNullOrEmpty(filePath))
                {
                    var jsonData = await _jsonFileService.LoadJsonFileAsync(filePath);
                    
                    if (jsonData.IsValidJson)
                    {
                        return Ok(new
                        {
                            success = true,
                            data = new
                            {
                                columnNames = jsonData.ColumnNames,
                                columnTypes = jsonData.ColumnTypes,
                                tableData = jsonData.TableData,
                                filePath = jsonData.FilePath,
                                fileName = jsonData.FileName,
                                isValidJson = jsonData.IsValidJson
                            }
                        });
                    }
                    else
                    {
                        return Ok(new { success = false, error = jsonData.ErrorMessage });
                    }
                }
                else
                {
                    // Return empty structure for new dictionary
                    return Ok(new
                    {
                        success = true,
                        data = new
                        {
                            columnNames = GetDefaultColumnsForDictionary(selectedFileType),
                            columnTypes = GetDefaultColumnTypesForDictionary(selectedFileType),
                            tableData = new List<Dictionary<string, object>>(),
                            filePath = filePath ?? GetDictionaryFilePath(selectedFileType),
                            fileName = GetDictionaryFileName(selectedFileType),
                            isValidJson = true
                        }
                    });
                }
            }
            
            return BadRequest(new { success = false, error = "Invalid file type" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while loading dictionary data" });
        }
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDictionary([FromForm] string filePath, [FromForm] string jsonData, [FromForm] int fileType)
    {
        try
        {
            PathValidator.SanitizePath(filePath);
            var tableData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonData);
            if (tableData != null && Enum.IsDefined(typeof(FileType), fileType))
            {
                var selectedFileType = (FileType)fileType;
                
                // Validate uniqueness before saving
                var fileName = GetDictionaryFileName(selectedFileType);
                var validationResult = await _validationService.ValidateUniquenessAsync(fileName, tableData);
                
                if (!validationResult.IsValid)
                {
                    return Ok(new
                    {
                        success = false,
                        error = "Cannot save: There are validation errors. Please fix duplicate values and try again.",
                        validationErrors = validationResult.Errors.Select(e => new
                        {
                            rowIndex = e.RowIndex,
                            fieldName = e.FieldName,
                            message = e.Message
                        }).ToList()
                    });
                }
                
                var success = await _jsonFileService.SaveJsonFileAsync(filePath, tableData);
                
                if (success)
                {
                    // Return success with existing data (avoid unnecessary file reload)
                    var columnNames = GetDefaultColumnsForDictionary(selectedFileType);
                    var columnTypes = GetDefaultColumnTypesForDictionary(selectedFileType);
                    
                    return Ok(new
                    {
                        success = true,
                        message = $"{GetDictionaryDisplayName(selectedFileType)} dictionary saved successfully!",
                        data = new
                        {
                            columnNames = columnNames,
                            columnTypes = columnTypes,
                            tableData = tableData,
                            filePath = filePath,
                            fileName = GetDictionaryFileName(selectedFileType),
                            isValidJson = true
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        error = "Failed to save changes."
                    });
                }
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Invalid data format."
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                error = "An error occurred while saving dictionary"
            });
        }
    }

    [HttpPost("upload")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadViewModel upload)
    {
        try
        {
            if (upload.FileType == FileType.None)
            {
                return BadRequest(new { success = false, error = "Please select a valid dictionary type." });
            }

            if (upload.JsonFile == null || upload.JsonFile.Length == 0)
            {
                return BadRequest(new { success = false, error = "Please select a JSON file to upload." });
            }

            if (!upload.JsonFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, error = "Please select a valid JSON file." });
            }

            // Directly overwrite the file
            var (success, message) = await _storageService.OverwriteFileAsync(upload);

            if (success)
            {
                return Ok(new { success = true, message });
            }
            else
            {
                return BadRequest(new { success = false, error = message });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while uploading file" });
        }
    }

    private string GetDictionaryFilePath(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => "data/dictionaries/templates.json",
            FileType.EventTriggers => "data/dictionaries/event-triggers.json",
            FileType.EventChannels => "data/dictionaries/event-channels.json",
            FileType.OrderTypes => "data/dictionaries/order-types.json",
            FileType.Customers => "data/dictionaries/customers.json",
            _ => string.Empty
        };
    }
    
    private string GetDictionaryFileName(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => "templates.json",
            FileType.EventTriggers => "event-triggers.json",
            FileType.EventChannels => "event-channels.json",
            FileType.OrderTypes => "order-types.json",
            FileType.Customers => "customers.json",
            _ => "unknown.json"
        };
    }
    
    private List<string> GetDefaultColumnsForDictionary(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => new List<string> { "templateId", "templateName", "channelType" },
            FileType.EventTriggers => new List<string> { "Event Name", "IsActive" },
            FileType.EventChannels => new List<string> { "Channel Name", "IsActive" },
            FileType.OrderTypes => new List<string> { "Order Type" },
            FileType.Customers => new List<string> { "customerId", "companyName", "contactPerson", "email", "phone", "address", "IsActive" },
            _ => new List<string> { "id", "name" }
        };
    }
    
    private Dictionary<string, string> GetDefaultColumnTypesForDictionary(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => new Dictionary<string, string> { { "templateId", "text" }, { "templateName", "text" }, { "channelType", "text" } },
            FileType.EventTriggers => new Dictionary<string, string> { { "Event Name", "text" }, { "IsActive", "boolean" } },
            FileType.EventChannels => new Dictionary<string, string> { { "Channel Name", "text" }, { "IsActive", "boolean" } },
            FileType.OrderTypes => new Dictionary<string, string> { { "Order Type", "text" } },
            FileType.Customers => new Dictionary<string, string> { { "customerId", "text" }, { "companyName", "text" }, { "contactPerson", "text" }, { "email", "text" }, { "phone", "text" }, { "address", "text" }, { "IsActive", "boolean" } },
            _ => new Dictionary<string, string> { { "id", "text" }, { "name", "text" } }
        };
    }
    
    private string GetDictionaryDisplayName(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => "Notification Templates",
            FileType.EventTriggers => "Event Triggers",
            FileType.EventChannels => "Event Channels",
            FileType.OrderTypes => "Order Types",
            FileType.Customers => "Customers",
            _ => "Dictionary"
        };
    }
}