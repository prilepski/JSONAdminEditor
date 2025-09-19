using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using System.Text.Json;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly NotificationsService _notificationsService;

    public EventsController(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [HttpGet("triggers")]
    public async Task<IActionResult> GetActiveEventTriggers()
    {
        try
        {
            var triggers = await _notificationsService.GetActiveEventTriggersAsync();
            return Ok(triggers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving event triggers" });
        }
    }

    [HttpGet("order-types")]
    public async Task<IActionResult> GetAvailableOrderTypes()
    {
        try
        {
            var orderTypes = await _notificationsService.GetAvailableOrderTypesAsync();
            return Ok(orderTypes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving order types" });
        }
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetAvailableTemplates()
    {
        try
        {
            var templates = await _notificationsService.GetAvailableTemplatesAsync();
            return Ok(templates.Select(t => new { templateId = t.TemplateId, templateName = t.TemplateName, channelType = t.ChannelType }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving templates" });
        }
    }

    [HttpGet("supports-order-type")]
    public async Task<IActionResult> CheckEventSupportsByOrderType(string eventName)
    {
        try
        {
            var supports = await _notificationsService.CheckEventSupportsByOrderTypeAsync(eventName);
            return Ok(supports);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while checking event support" });
        }
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetEventByNameAndOrderType(string eventName, string orderType)
    {
        try
        {
            var eventData = await _notificationsService.GetEventByNameAndOrderTypeAsync(eventName, orderType);
            return Ok(eventData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving event data" });
        }
    }

    [HttpGet("template")]
    public async Task<IActionResult> GetEventTemplate()
    {
        try
        {
            var template = await _notificationsService.GetEventTemplateAsync();
            return Ok(template);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving event template" });
        }
    }

    public class EventRequest
    {
        public string eventName { get; set; } = string.Empty;
        public string? orderType { get; set; }
        public Dictionary<string, object> eventData { get; set; } = new();
        public bool isNew { get; set; }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveEvent([FromBody] JsonElement requestData)
    {
        try
        {
            var request = JsonSerializer.Deserialize<EventRequest>(requestData.GetRawText());
            if (request == null) return BadRequest(new { success = false, error = "Invalid request data" });
            
            bool success;
            if (request.isNew)
            {
                success = await _notificationsService.AddNewEventAsync(request.eventName, request.eventData);
            }
            else
            {
                if (string.IsNullOrEmpty(request.orderType))
                {
                    return BadRequest(new { success = false, error = "OrderType is required for updating existing events" });
                }
                success = await _notificationsService.UpdateEventByOrderTypeAsync(request.eventName, request.orderType, request.eventData);
            }
            
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving event" });
        }
    }
}