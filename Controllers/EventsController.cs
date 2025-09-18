using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;

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
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
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
            return StatusCode(500, new { error = ex.Message });
        }
    }

    public class EventRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string? OrderType { get; set; }
        public Dictionary<string, object> EventData { get; set; } = new();
        public bool IsNew { get; set; }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveEvent([FromBody] EventRequest request)
    {
        try
        {
            bool success;
            if (request.IsNew)
            {
                success = await _notificationsService.AddNewEventAsync(request.EventName, request.EventData);
            }
            else
            {
                success = await _notificationsService.UpdateEventByOrderTypeAsync(request.EventName, request.OrderType, request.EventData);
            }
            
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}