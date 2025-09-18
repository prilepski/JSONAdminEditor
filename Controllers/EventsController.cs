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
            return Ok(templates.Select(t => new { templateId = t.TemplateId, templateName = t.TemplateName }));
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

    [HttpPost("add")]
    public async Task<IActionResult> AddNewEvent([FromBody] dynamic request)
    {
        try
        {
            string eventName = request.eventName;
            var eventData = request.eventData;
            
            var success = await _notificationsService.AddNewEventAsync(eventName, eventData);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateEventByOrderType([FromBody] dynamic request)
    {
        try
        {
            string eventName = request.eventName;
            string orderType = request.orderType;
            var eventData = request.eventData;
            
            var success = await _notificationsService.UpdateEventByOrderTypeAsync(eventName, orderType, eventData);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}