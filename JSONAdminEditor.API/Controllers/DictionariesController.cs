namespace JSONAdminEditor.Controllers;

using JSONAdminEditor.API.Constants;
using JSONAdminEditor.API.Exceptions;
using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Application.Models.Dictionaries;
using JSONAdminEditor.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/dictionaries")]
[Produces("application/json")]
public class DictionariesController(IFileContentService fileContentService, IJsonFileService jsonFileService) : ControllerBase
{
    private readonly IFileContentService _fileContentService = fileContentService;
    private readonly IJsonFileService _jsonFileService = jsonFileService;

    // Templates endpoints
    [HttpGet("templates")]
    [ProducesResponseType(200, Type = typeof(List<Template>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Template>>> GetTemplates()
    {
        return await GetDictionaryData<Template>(FileType.Templates);
    }

    [HttpPut("templates")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateTemplates([FromBody] List<Template> templates)
    {
        if (Validator.HasUniqueKeys(templates, x => x.TemplateId))
        {
            return BadRequest("Templates contains duplication");
        }

        return await SaveDictionaryData(templates, FileType.Templates);
    }

    // Event Triggers endpoints
    [HttpGet("event-triggers")]
    [ProducesResponseType(200, Type = typeof(List<EventTrigger>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventTrigger>>> GetEventTriggers()
    {
        return await GetDictionaryData<EventTrigger>(FileType.EventTriggers);
    }

    [HttpPut("event-triggers")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventTriggers([FromBody] List<EventTrigger> eventTriggers)
    {
        if (Validator.HasUniqueKeys(eventTriggers, x => x.EventName))
        {
            return BadRequest("Event triggers contains duplication");
        }

        return await SaveDictionaryData(eventTriggers, FileType.EventTriggers);
    }

    [HttpGet("event-channels")]
    [ProducesResponseType(200, Type = typeof(List<EventChannel>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventChannel>>> GetEventChannels()
    {
        return await GetDictionaryData<EventChannel>(FileType.EventChannels);
    }

    [HttpPut("event-channels")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventChannels([FromBody] List<EventChannel> eventChannels)
    {
        if (Validator.HasUniqueKeys(eventChannels, x => x.ChannelName))
        {
            return BadRequest("Event channels contains duplication");
        }

        return await SaveDictionaryData(eventChannels, FileType.EventChannels);
    }

    [HttpGet("order-types")]
    [ProducesResponseType(200, Type = typeof(List<OrderType>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<OrderType>>> GetOrderTypes()
    {
        return await GetDictionaryData<OrderType>(FileType.OrderTypes);
    }

    [HttpPut("order-types")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateOrderTypes([FromBody] List<OrderType> orderTypes)
    {
        if (Validator.HasUniqueKeys(orderTypes, x => x.Name))
        {
            return BadRequest("Order types contains duplication");
        }

        return await SaveDictionaryData(orderTypes, FileType.OrderTypes);
    }

    [HttpGet("customers")]
    [ProducesResponseType(200, Type = typeof(List<Customer>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Customer>>> GetCustomers()
    {
        return await GetDictionaryData<Customer>(FileType.Customers);
    }

    [HttpPut("customers")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomers([FromBody] List<Customer> customers)
    {
        if (Validator.HasUniqueKeys(customers, x => x.CustomerId))
        {
            return BadRequest("Customers contains duplication");
        }

        return await SaveDictionaryData(customers, FileType.Customers);
    }

    [HttpGet("logo-url")]
    [ProducesResponseType(200, Type = typeof(List<LogoUrl>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<LogoUrl>>> GetLogoUrlMappings()
    {
        return await GetDictionaryData<LogoUrl>(FileType.LogoUrlMappings);
    }

    [HttpPut("logo-url")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateLogoUrlMappings([FromBody] List<LogoUrl> logoUrls)
    {
        if (Validator.HasUniqueKeys(logoUrls, x => x.FileName))
        {
            return BadRequest("Logo urls contains duplication");
        }

        return await SaveDictionaryData(logoUrls, FileType.LogoUrlMappings);
    }


    // File Upload endpoint (generic for all dictionary types)
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadViewModel upload)
    {
        if (upload == null)
            return BadRequest("Upload data is required");

        if (upload.FileType == FileType.None)
            return BadRequest("Please select a valid dictionary type");

        if (upload.JsonFile == null || upload.JsonFile.Length == 0)
            return BadRequest("Please select a JSON file to upload");

        if (!upload.JsonFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Please select a valid JSON file");

        using var stream = upload.JsonFile.OpenReadStream();
        using var reader = new StreamReader(stream);
        var jsonContent = await reader.ReadToEndAsync();

        await SaveTypedDictionaryData(upload.FileType, jsonContent);
        return NoContent();
    }

    private async Task<ActionResult<List<T>>> GetDictionaryData<T>(FileType fileType) where T : class, new()
    {
        string filePath = GetFilePathForType(fileType);

        try
        {
            var data = await _jsonFileService.LoadJsonFileAsync<List<T>>(filePath);
            return Ok(data ?? new List<T>());
        }
        catch (JsonFileException ex)
        {
            return StatusCode(500, $"Error loading data: {ex.Message}");
        }
    }

    private async Task<IActionResult> SaveDictionaryData<T>(List<T> data, FileType fileType)
    {
        string dataType = GetFileTypeDisplay(fileType);

        if (data == null)
            return BadRequest($"{dataType} data is required");

        if (!ModelState.IsValid)
            return BadRequest($"Invalid {dataType.ToLower()} data");

        string filePath = GetFilePathForType(fileType);

        try
        {
            await _jsonFileService.SaveJsonFileAsync(filePath, data);
            return NoContent();
        }
        catch (JsonFileException ex)
        {
            return StatusCode(500, $"Error saving {dataType.ToLower()}: {ex.Message}");
        }
    }

    private async Task SaveTypedDictionaryData(FileType fileType, string jsonData)
    {
        await _fileContentService.WriteFileAsync(GetFilePathForType(fileType), jsonData);
    }

    private static string GetFileTypeDisplay(FileType fileType) => fileType switch
    {
        FileType.None => "None",
        FileType.Templates => "Notification Templates",
        FileType.EventTriggers => "Event Triggers",
        FileType.EventChannels => "Event Channels",
        FileType.OrderTypes => "Order Types",
        FileType.Customers => "Customers",
        FileType.LogoUrlMappings => "Logo URLs",
        FileType.CustomerSettings => "Customer Override",
        _ => fileType.ToString()
    };

    private static string GetFilePathForType(FileType fileType) => fileType switch
    {
        FileType.Templates => FilePaths.Templates,
        FileType.EventTriggers => FilePaths.EventTriggers,
        FileType.EventChannels => FilePaths.EventChannels,
        FileType.OrderTypes => FilePaths.OrderTypes,
        FileType.Customers => FilePaths.Customers,
        FileType.LogoUrlMappings => FilePaths.LogoUrlMappings,
        _ => throw new ArgumentException($"Unknown file type: {fileType}")
    };
}