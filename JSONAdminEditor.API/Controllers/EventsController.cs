using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly NotificationsService _notificationsService;
    private readonly IMockDatabaseService _mockDb;

    public EventsController(NotificationsService notificationsService, IMockDatabaseService mockDb)
    {
        _notificationsService = notificationsService;
        _mockDb = mockDb;
    }

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
            // Filter templates that contain the order type OR are generic (don't contain Pickup/Delivery)
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

    [HttpGet("data")]
    public async Task<IActionResult> GetEventByNameAndOrderType(string eventName, string orderType)
    {
        var eventData = await _notificationsService.GetEventByNameAndOrderTypeAsync(eventName, orderType);
        return Ok(eventData);
    }

    [HttpGet("template")]
    public async Task<IActionResult> GetEventTemplate()
    {
        var template = await _notificationsService.GetEventTemplateAsync();
        return Ok(template);
    }

    public class EventRequest
    {
        public string eventName { get; set; } = string.Empty;
        public string? orderType { get; set; }
        public EventMapping eventData { get; set; } = new();
        public bool isNew { get; set; }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveEvent([FromBody] EventRequest request)
    {
        if (request == null) 
            return BadRequest(new { success = false, error = "Event request data is required" });
        
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid event data" });
        
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
        
        if (success)
        {
            return Ok(new { success = true, message = "Event saved successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to save event" });
        }
    }
}