using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using System.Text.Json;
using JSONAdminEditor.Application.Models.Dictionaries;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/dictionaries")]
[Produces("application/json")]
public class DictionariesController : ControllerBase
{
    private readonly IFileContentService _fileContentService;
    private readonly JsonSerializerOptions _jsonOptions;

    public DictionariesController(IFileContentService fileContentService)
    {
        _fileContentService = fileContentService;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
    }

    // Templates endpoints
    [HttpGet("templates")]
    [ProducesResponseType(200, Type = typeof(List<Template>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Template>>> GetTemplates()
    {
        return await GetDictionaryData<Template>(GetFilePathForType(FileType.Templates));
    }

    [HttpPut("templates")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateTemplates([FromBody] List<Template> templates)
    {
        return await SaveDictionaryData(templates, GetFilePathForType(FileType.Templates), "Templates");
    }

    // Event Triggers endpoints
    [HttpGet("event-triggers")]
    [ProducesResponseType(200, Type = typeof(List<EventTrigger>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventTrigger>>> GetEventTriggers()
    {
        return await GetDictionaryData<EventTrigger>(GetFilePathForType(FileType.EventTriggers));
    }

    [HttpPut("event-triggers")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventTriggers([FromBody] List<EventTrigger> eventTriggers)
    {
        return await SaveDictionaryData(eventTriggers, GetFilePathForType(FileType.EventTriggers), "Event triggers");
    }

    [HttpGet("event-channels")]
    [ProducesResponseType(200, Type = typeof(List<EventChannel>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventChannel>>> GetEventChannels()
    {
        return await GetDictionaryData<EventChannel>(GetFilePathForType(FileType.EventChannels));
    }

    [HttpPut("event-channels")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventChannels([FromBody] List<EventChannel> eventChannels)
    {
        return await SaveDictionaryData(eventChannels, GetFilePathForType(FileType.EventChannels), "Event channels");
    }

    [HttpGet("order-types")]
    [ProducesResponseType(200, Type = typeof(List<OrderType>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<OrderType>>> GetOrderTypes()
    {
        return await GetDictionaryData<OrderType>(GetFilePathForType(FileType.OrderTypes));
    }

    [HttpPut("order-types")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateOrderTypes([FromBody] List<OrderType> orderTypes)
    {
        return await SaveDictionaryData(orderTypes, GetFilePathForType(FileType.OrderTypes), "Order types");
    }

    [HttpGet("customers")]
    [ProducesResponseType(200, Type = typeof(List<Customer>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Customer>>> GetCustomers()
    {
        return await GetDictionaryData<Customer>(GetFilePathForType(FileType.Customers));
    }

    [HttpPut("customers")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomers([FromBody] List<Customer> customers)
    {
        return await SaveDictionaryData(customers, GetFilePathForType(FileType.Customers), "Customers");
    }

    [HttpGet("logo-url")]
    [ProducesResponseType(200, Type = typeof(List<LogoUrl>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<LogoUrl>>> GetLogoUrlMappings()
    {
        return await GetDictionaryData<LogoUrl>(GetFilePathForType(FileType.LogoUrlMappings));
    }

    [HttpPut("logo-url")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateLogoUrlMappings([FromBody] List<LogoUrl> logoUrls)
    {
        return await SaveDictionaryData(logoUrls, GetFilePathForType(FileType.LogoUrlMappings), "Logo URL mappings");
    }

    private async Task<ActionResult<List<T>>> GetDictionaryData<T>(string filePath)
    {
        var content = await _fileContentService.ReadFileAsync(filePath);
        if (string.IsNullOrEmpty(content))
            return Ok(new List<T>());

        var data = JsonSerializer.Deserialize<List<T>>(content, _jsonOptions);
        return Ok(data ?? new List<T>());
    }

    private async Task<IActionResult> SaveDictionaryData<T>(List<T> data, string filePath, string dataType)
    {
        if (data == null)
            return BadRequest($"{dataType} data is required");

        if (!ModelState.IsValid)
            return BadRequest($"Invalid {dataType.ToLower()} data");

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        await _fileContentService.WriteFileAsync(filePath, json);
        return NoContent();
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

    private async Task SaveTypedDictionaryData(FileType fileType, string jsonData)
    {
        await _fileContentService.WriteFileAsync(GetFilePathForType(fileType), jsonData);
    }

    private static string GetFilePathForType(FileType fileType) => fileType switch
    {
        FileType.Templates => "data/dictionaries/templates.json",
        FileType.EventTriggers => "data/dictionaries/event-triggers.json",
        FileType.EventChannels => "data/dictionaries/event-channels.json",
        FileType.OrderTypes => "data/dictionaries/order-types.json",
        FileType.Customers => "data/dictionaries/customers.json",
        FileType.LogoUrlMappings => "data/dictionaries/logo-url-mappings.json",
        _ => throw new ArgumentException($"Unknown file type: {fileType}")
    };
}