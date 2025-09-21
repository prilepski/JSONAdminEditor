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

    // Helper methods for event helper endpoints (keeping existing functionality)
    [HttpGet("triggers")]
    [ProducesResponseType(200, Type = typeof(List<EventTrigger>))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetActiveEventTriggers()
    {
        var triggers = await _notificationsService.GetActiveEventTriggersAsync();
        return Ok(triggers);
    }

    [HttpGet("available-order-types")]
    [ProducesResponseType(200, Type = typeof(List<string>))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAvailableOrderTypes()
    {
        var orderTypes = await _mockDb.GetOrderTypesAsync();
        return Ok(orderTypes.Select(ot => ot.Name));
    }

    [HttpGet("available-templates")]
    [ProducesResponseType(200, Type = typeof(List<object>))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetAvailableTemplates([FromQuery] string? orderType = null)
    {
        var templates = await _mockDb.GetTemplatesAsync();
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
    [ProducesResponseType(200, Type = typeof(bool))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CheckEventSupportsByOrderType(string eventName)
    {
        var supports = await _notificationsService.CheckEventSupportsByOrderTypeAsync(eventName);
        return Ok(supports);
    }

    [HttpGet("event-data")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetEventByNameAndOrderType(string eventName, string orderType)
    {
        var eventData = await _notificationsService.GetEventByNameAndOrderTypeAsync(eventName, orderType);
        return Ok(eventData);
    }

    [HttpGet("event-template")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetEventTemplate()
    {
        var template = await _notificationsService.GetEventTemplateAsync();
        return Ok(template);
    }

    private async Task SaveTypedDictionaryData(FileType fileType, string jsonData)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        switch (fileType)
        {
            case FileType.Templates:
                await _mockDb.SaveTemplatesAsync(JsonSerializer.Deserialize<List<Template>>(jsonData, options) ?? new());
                break;
            case FileType.EventTriggers:
                await _mockDb.SaveEventTriggersAsync(JsonSerializer.Deserialize<List<EventTrigger>>(jsonData, options) ?? new());
                break;
            case FileType.EventChannels:
                await _mockDb.SaveEventChannelsAsync(JsonSerializer.Deserialize<List<EventChannel>>(jsonData, options) ?? new());
                break;
            case FileType.OrderTypes:
                await _mockDb.SaveOrderTypesAsync(JsonSerializer.Deserialize<List<OrderType>>(jsonData, options) ?? new());
                break;
            case FileType.Customers:
                await _mockDb.SaveCustomersAsync(JsonSerializer.Deserialize<List<Customer>>(jsonData, options) ?? new());
                break;
        }
    }
}