using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Models;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using System.Text.Json;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/dictionaries")]
[Produces("application/json")]
public class DictionariesController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;
    private readonly NotificationsService _notificationsService;

    public DictionariesController(IMockDatabaseService mockDb, NotificationsService notificationsService)
    {
        _mockDb = mockDb;
        _notificationsService = notificationsService;
    }

    // Templates endpoints
    [HttpGet("templates")]
    [ProducesResponseType(200, Type = typeof(List<Template>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Template>>> GetTemplates()
    {
        var templates = await _mockDb.GetTemplatesAsync();
        return Ok(templates);
    }

    [HttpPut("templates")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateTemplates([FromBody] List<Template> templates)
    {
        if (templates == null)
            return BadRequest("Templates data is required");

        if (!ModelState.IsValid)
            return BadRequest("Invalid templates data");

        await _mockDb.SaveTemplatesAsync(templates);
        return NoContent();
    }

    // Event Triggers endpoints
    [HttpGet("event-triggers")]
    [ProducesResponseType(200, Type = typeof(List<EventTrigger>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventTrigger>>> GetEventTriggers()
    {
        var eventTriggers = await _mockDb.GetEventTriggersAsync();
        return Ok(eventTriggers);
    }

    [HttpPut("event-triggers")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventTriggers([FromBody] List<EventTrigger> eventTriggers)
    {
        if (eventTriggers == null)
            return BadRequest("Event triggers data is required");

        if (!ModelState.IsValid)
            return BadRequest("Invalid event triggers data");

        await _mockDb.SaveEventTriggersAsync(eventTriggers);
        return NoContent();
    }

    // Event Channels endpoints
    [HttpGet("event-channels")]
    [ProducesResponseType(200, Type = typeof(List<EventChannel>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventChannel>>> GetEventChannels()
    {
        var eventChannels = await _mockDb.GetEventChannelsAsync();
        return Ok(eventChannels);
    }

    [HttpPut("event-channels")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventChannels([FromBody] List<EventChannel> eventChannels)
    {
        if (eventChannels == null)
            return BadRequest("Event channels data is required");

        if (!ModelState.IsValid)
            return BadRequest("Invalid event channels data");

        await _mockDb.SaveEventChannelsAsync(eventChannels);
        return NoContent();
    }

    // Order Types endpoints
    [HttpGet("order-types")]
    [ProducesResponseType(200, Type = typeof(List<OrderType>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<OrderType>>> GetOrderTypes()
    {
        var orderTypes = await _mockDb.GetOrderTypesAsync();
        return Ok(orderTypes);
    }

    [HttpPut("order-types")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateOrderTypes([FromBody] List<OrderType> orderTypes)
    {
        if (orderTypes == null)
            return BadRequest("Order types data is required");

        if (!ModelState.IsValid)
            return BadRequest("Invalid order types data");

        await _mockDb.SaveOrderTypesAsync(orderTypes);
        return NoContent();
    }

    // Customers endpoints
    [HttpGet("customers")]
    [ProducesResponseType(200, Type = typeof(List<Customer>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Customer>>> GetCustomers()
    {
        var customers = await _mockDb.GetCustomersAsync();
        return Ok(customers);
    }

    [HttpPut("customers")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateCustomers([FromBody] List<Customer> customers)
    {
        if (customers == null)
            return BadRequest("Customers data is required");

        if (!ModelState.IsValid)
            return BadRequest("Invalid customers data");

        await _mockDb.SaveCustomersAsync(customers);
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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        switch (fileType)
        {
            case FileType.Templates:
                var templates = JsonSerializer.Deserialize<List<Template>>(jsonData, options);
                if (templates != null) await _mockDb.SaveTemplatesAsync(templates);
                break;
            case FileType.EventTriggers:
                var eventTriggers = JsonSerializer.Deserialize<List<EventTrigger>>(jsonData, options);
                if (eventTriggers != null) await _mockDb.SaveEventTriggersAsync(eventTriggers);
                break;
            case FileType.EventChannels:
                var eventChannels = JsonSerializer.Deserialize<List<EventChannel>>(jsonData, options);
                if (eventChannels != null) await _mockDb.SaveEventChannelsAsync(eventChannels);
                break;
            case FileType.OrderTypes:
                var orderTypes = JsonSerializer.Deserialize<List<OrderType>>(jsonData, options);
                if (orderTypes != null) await _mockDb.SaveOrderTypesAsync(orderTypes);
                break;
            case FileType.Customers:
                var customers = JsonSerializer.Deserialize<List<Customer>>(jsonData, options);
                if (customers != null) await _mockDb.SaveCustomersAsync(customers);
                break;
        }
    }
}