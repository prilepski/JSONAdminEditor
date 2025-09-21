using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using JSONAdminEditor.Security;
using JSONAdminEditor.Application.Models;
using System.Text.Json;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/dictionaries")]
public class DictionariesController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;
    private readonly NotificationsService _notificationsService;

    public DictionariesController(IMockDatabaseService mockDb, NotificationsService notificationsService)
    {
        _mockDb = mockDb;
        _notificationsService = notificationsService;
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetDictionaryData(int fileType)
    {
        if (!Enum.IsDefined(typeof(FileType), fileType))
            return BadRequest(new { success = false, error = "Invalid file type" });
            
        var selectedFileType = (FileType)fileType;
        var filePath = GetDictionaryFilePath(selectedFileType);
        
        if (!string.IsNullOrEmpty(filePath))
        {
            var tableData = await _mockDb.GetDictionaryAsync(GetDictionaryKey(selectedFileType));
            
            if (tableData != null)
            {
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        columnNames = GetDefaultColumnsForDictionary(selectedFileType),
                        columnTypes = GetDefaultColumnTypesForDictionary(selectedFileType),
                        tableData = tableData,
                        filePath = filePath,
                        fileName = GetDictionaryFileName(selectedFileType),
                        isValidJson = true
                    }
                });
            }
            else
            {
                return Ok(new { success = false, error = "Dictionary not found" });
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
                    filePath = GetDictionaryFilePath(selectedFileType),
                    fileName = GetDictionaryFileName(selectedFileType),
                    isValidJson = true
                }
            });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveDictionary([FromForm] string filePath, [FromForm] string jsonData, [FromForm] int fileType)
    {
        if (string.IsNullOrEmpty(filePath))
            return BadRequest(new { success = false, error = "File path is required" });
            
        if (string.IsNullOrEmpty(jsonData))
            return BadRequest(new { success = false, error = "JSON data is required" });
            
        if (!Enum.IsDefined(typeof(FileType), fileType))
            return BadRequest(new { success = false, error = "Invalid file type" });
            
        PathValidator.SanitizePath(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var tableData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonData, options);
        
        // Convert JsonElement to proper values
        var convertedData = tableData?.Select(row => 
            row.ToDictionary(kvp => kvp.Key, kvp => GetJsonElementValue(kvp.Value))
        ).ToList();
        
        if (convertedData == null)
            return BadRequest(new { success = false, error = "Invalid data format" });
            
        var selectedFileType = (FileType)fileType;
        var success = await _mockDb.SaveDictionaryAsync(GetDictionaryKey(selectedFileType), convertedData);
        
        if (success)
        {
            return Ok(new
            {
                success = true,
                message = $"{GetDictionaryDisplayName(selectedFileType)} dictionary saved successfully!",
                data = new
                {
                    columnNames = GetDefaultColumnsForDictionary(selectedFileType),
                    columnTypes = GetDefaultColumnTypesForDictionary(selectedFileType),
                    tableData = convertedData,
                    filePath = filePath,
                    fileName = GetDictionaryFileName(selectedFileType),
                    isValidJson = true
                }
            });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to save dictionary" });
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadViewModel upload)
    {
        if (upload == null)
            return BadRequest(new { success = false, error = "Upload data is required" });
            
        if (upload.FileType == FileType.None)
            return BadRequest(new { success = false, error = "Please select a valid dictionary type" });

        if (upload.JsonFile == null || upload.JsonFile.Length == 0)
            return BadRequest(new { success = false, error = "Please select a JSON file to upload" });

        if (!upload.JsonFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { success = false, error = "Please select a valid JSON file" });

        // Mock upload - always return success
        var success = true;
        
        if (success)
        {
            return Ok(new { success = true, message = "File uploaded successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to upload file" });
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
    
    private static object GetJsonElementValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.TryGetInt32(out var intVal) ? intVal : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
    
    private static string GetDictionaryKey(FileType fileType)
    {
        return fileType switch
        {
            FileType.Templates => "templates",
            FileType.EventTriggers => "event-triggers",
            FileType.EventChannels => "event-channels",
            FileType.OrderTypes => "order-types",
            FileType.Customers => "customers",
            _ => "unknown"
        };
    }

    // Event Helper Endpoints
    [HttpGet("triggers")]
    public async Task<IActionResult> GetActiveEventTriggers()
    {
        var triggers = await _notificationsService.GetActiveEventTriggersAsync();
        return Ok(triggers);
    }

    [HttpGet("order-types")]
    public async Task<IActionResult> GetAvailableOrderTypes()
    {
        var orderTypes = await _notificationsService.GetAvailableOrderTypesAsync();
        return Ok(orderTypes);
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetAvailableTemplates([FromQuery] string? orderType = null)
    {
        var templates = await _notificationsService.GetAvailableTemplatesAsync();
        var filteredTemplates = templates;
        
        if (!string.IsNullOrEmpty(orderType))
        {
            filteredTemplates = templates.Where(t => 
                t.TemplateName.Contains(orderType, StringComparison.OrdinalIgnoreCase) ||
                (!t.TemplateName.Contains("Pickup", StringComparison.OrdinalIgnoreCase) && 
                 !t.TemplateName.Contains("Delivery", StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }
        
        return Ok(filteredTemplates.Select(t => new { templateId = t.TemplateId, templateName = t.TemplateName, channelType = t.ChannelType }));
    }

    [HttpGet("supports-order-type")]
    public async Task<IActionResult> CheckEventSupportsByOrderType(string eventName)
    {
        var supports = await _notificationsService.CheckEventSupportsByOrderTypeAsync(eventName);
        return Ok(supports);
    }

    [HttpGet("event-data")]
    public async Task<IActionResult> GetEventByNameAndOrderType(string eventName, string orderType)
    {
        var eventData = await _notificationsService.GetEventByNameAndOrderTypeAsync(eventName, orderType);
        return Ok(eventData);
    }

    [HttpGet("event-template")]
    public async Task<IActionResult> GetEventTemplate()
    {
        var template = await _notificationsService.GetEventTemplateAsync();
        return Ok(template);
    }
}