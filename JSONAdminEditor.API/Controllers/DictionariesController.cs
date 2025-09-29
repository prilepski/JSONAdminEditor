namespace JSONAdminEditor.Controllers;

using JSONAdminEditor.API.Exceptions;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Application.Models.Dictionaries;
using JSONAdminEditor.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/dictionaries")]
[Produces("application/json")]
public class DictionariesController(IConfigRepository repository) : ControllerBase
{
    private readonly IConfigRepository _repository = repository;

    // Templates endpoints
    [HttpGet("templates")]
    [ProducesResponseType(200, Type = typeof(List<Template>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Template>>> GetTemplates()
    {
        return await _repository.GetDictionaryDataAsync<Template>(FileType.DictionaryTemplates);
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

        return await SaveDictionaryData(templates, FileType.DictionaryTemplates);
    }

    // Event Triggers endpoints
    [HttpGet("event-triggers")]
    [ProducesResponseType(200, Type = typeof(List<EventTrigger>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventTrigger>>> GetEventTriggers()
    {
        return await _repository.GetDictionaryDataAsync<EventTrigger>(FileType.DictionaryEventTriggers);
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

        return await SaveDictionaryData(eventTriggers, FileType.DictionaryEventTriggers);
    }

    [HttpGet("event-channels")]
    [ProducesResponseType(200, Type = typeof(List<EventChannel>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventChannel>>> GetEventChannels()
    {
        return await _repository.GetDictionaryDataAsync<EventChannel>(FileType.DictionaryEventChannels);
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

        return await SaveDictionaryData(eventChannels, FileType.DictionaryEventChannels);
    }

    [HttpGet("order-types")]
    [ProducesResponseType(200, Type = typeof(List<OrderType>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<OrderType>>> GetOrderTypes()
    {
        return await _repository.GetDictionaryDataAsync<OrderType>(FileType.DictionaryOrderTypes);
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

        return await SaveDictionaryData(orderTypes, FileType.DictionaryOrderTypes);
    }

    [HttpGet("customers")]
    [ProducesResponseType(200, Type = typeof(List<Customer>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Customer>>> GetCustomers()
    {
        return await _repository.GetDictionaryDataAsync<Customer>(FileType.DictionaryCustomers);
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

        return await SaveDictionaryData(customers, FileType.DictionaryCustomers);
    }

    [HttpGet("logo-url")]
    [ProducesResponseType(200, Type = typeof(List<LogoUrl>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<LogoUrl>>> GetLogoUrlMappings()
    {
        return await _repository.GetDictionaryDataAsync<LogoUrl>(FileType.DictionaryLogoUrls);
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

        return await SaveDictionaryData(logoUrls, FileType.DictionaryLogoUrls);
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

        await _repository.SaveDictionaryRawDataAsync(upload.FileType, jsonContent);
        return NoContent();
    }

    private async Task<IActionResult> SaveDictionaryData<T>(List<T> data, FileType fileType)
    {
        string dataType = GetFileTypeDisplay(fileType);

        if (data == null)
            return BadRequest($"{dataType} data is required");

        if (!ModelState.IsValid)
            return BadRequest($"Invalid {dataType.ToLower()} data");

        try
        {
            await _repository.SaveDictionaryDataAsync(fileType, data);
            return NoContent();
        }
        catch (JsonFileException ex)
        {
            return StatusCode(500, $"Error saving {dataType.ToLower()}: {ex.Message}");
        }
    }

    private static string GetFileTypeDisplay(FileType fileType) => fileType switch
    {
        FileType.None => "None",
        FileType.DictionaryTemplates => "Notification Templates",
        FileType.DictionaryEventTriggers => "Event Triggers",
        FileType.DictionaryEventChannels => "Event Channels",
        FileType.DictionaryOrderTypes => "Order Types",
        FileType.DictionaryCustomers => "Customers",
        FileType.DictionaryLogoUrls => "Logo URLs",
        _ => fileType.ToString()
    };
}