using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/events")]
[Produces("application/json")]
public class ConfigEventsController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public ConfigEventsController(IMockDatabaseService mockDb)
    {
        _mockDb = mockDb;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<EventMapping>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventMapping>>> GetEvents()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config.EventMappings);
    }

    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(EventMapping))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EventMapping?>> GetEvent(string eventName, string orderType)
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        var eventMapping = config.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (eventMapping == null)
            return NotFound(new { error = "Event mapping not found" });

        return Ok(eventMapping);
    }

    [HttpPut("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEvent(string eventName, string orderType, [FromBody] EventMapping eventMapping)
    {
        if (eventMapping == null)
            return BadRequest(new { success = false, error = "Event mapping data is required" });

        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid event mapping data" });

        var config = await _mockDb.GetNotificationMappingAsync();
        var existingIndex = config.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        // Ensure the event mapping has correct event name and order type
        eventMapping.Event = eventName;
        eventMapping.OrderType = orderType;

        if (existingIndex >= 0)
        {
            config.EventMappings[existingIndex] = eventMapping;
        }
        else
        {
            config.EventMappings.Add(eventMapping);
        }

        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "Event mapping updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update event mapping" });
        }
    }

    [HttpDelete("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteEvent(string eventName, string orderType)
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        var existingIndex = config.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (existingIndex < 0)
            return NotFound(new { success = false, error = "Event mapping not found" });

        config.EventMappings.RemoveAt(existingIndex);
        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "Event mapping deleted successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to delete event mapping" });
        }
    }

    // Event-specific nested properties
    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}/preferred-communication")]
    [ProducesResponseType(200, Type = typeof(List<PreferredCommunication>))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<PreferredCommunication>>> GetEventPreferredCommunication(string eventName, string orderType)
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        var eventMapping = config.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (eventMapping == null)
            return NotFound(new { error = "Event mapping not found" });

        return Ok(eventMapping.PreferredCommunication ?? new List<PreferredCommunication>());
    }

    [HttpPut("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}/preferred-communication")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventPreferredCommunication(string eventName, string orderType, [FromBody] List<PreferredCommunication> data)
    {
        if (data == null)
            return BadRequest(new { success = false, error = "Preferred communication data is required" });

        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid preferred communication data" });

        var config = await _mockDb.GetNotificationMappingAsync();
        var eventMapping = config.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (eventMapping == null)
            return NotFound(new { success = false, error = "Event mapping not found" });

        eventMapping.PreferredCommunication = data;
        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "Event preferred communication updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update event preferred communication" });
        }
    }

    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}/content-variables")]
    [ProducesResponseType(200, Type = typeof(List<object>))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<object>>> GetEventContentVariables(string eventName, string orderType)
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        var eventMapping = config.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (eventMapping == null)
            return NotFound(new { error = "Event mapping not found" });

        var contentVars = eventMapping.ContentVariables ?? new Dictionary<string, string>();
        var listData = contentVars.Select(kvp => new { key = kvp.Key, value = kvp.Value }).ToList();
        return Ok(listData);
    }

    [HttpPut("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}/content-variables")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEventContentVariables(string eventName, string orderType, [FromBody] Dictionary<string, string> contentVariables)
    {
        if (contentVariables == null)
            return BadRequest(new { success = false, error = "Content variables data is required" });

        var config = await _mockDb.GetNotificationMappingAsync();
        var eventMapping = config.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (eventMapping == null)
            return NotFound(new { success = false, error = "Event mapping not found" });

        eventMapping.ContentVariables = contentVariables;
        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "Event content variables updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update event content variables" });
        }
    }
}