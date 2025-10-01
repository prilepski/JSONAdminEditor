using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Domain.Entities;
using JSONAdminEditor.Domain.Interfaces;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/events")]
[Produces("application/json")]
public class ConfigEventsController(IConfigService configService) : ControllerBase
{
    private readonly IConfigService _configService = configService;

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<EventMapping>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventMapping>>> GetEvents()
    {
        var config = await _configService.GetGlobalConfigAsync();
        return Ok(config.EventMappings);
    }

    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(EventMapping))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EventMapping>> GetEvent(string eventName, string orderType)
    {
        var config = await _configService.GetGlobalConfigAsync();
        var eventMapping = FindEventMapping(config, eventName, orderType);
        
        return Ok(eventMapping ?? new EventMapping { Event = eventName, OrderType = orderType });
    }

    [HttpPut("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateEvent(string eventName, string orderType, [FromBody] EventMapping eventMapping)
    {
        if (eventMapping == null)
            return BadRequest("Event mapping data is required");

        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid event mapping data");

        var config = await _configService.GetGlobalConfigAsync();
        var existingIndex = FindIndex(config, eventName, orderType);

        eventMapping.Event = eventName;
        eventMapping.OrderType = orderType;

        if (existingIndex >= 0)
            config.EventMappings[existingIndex] = eventMapping;
        else
            config.EventMappings.Add(eventMapping);

        await _configService.SaveGlobalConfigAsync(config);
        return NoContent();
    }

    [HttpDelete("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteEvent(string eventName, string orderType)
    {
        var config = await _configService.GetGlobalConfigAsync();
        var existingIndex = FindIndex(config, eventName, orderType);

        if (existingIndex < 0)
            return NotFound("Event mapping not found");

        config.EventMappings.RemoveAt(existingIndex);

        await _configService.SaveGlobalConfigAsync(config);
        return NoContent();
    }

    private static EventMapping? FindEventMapping(NotificationMapping config, string eventName, string orderType) =>
        config.EventMappings.FirstOrDefault(e =>
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

    private static int FindIndex(NotificationMapping config, string eventName, string orderType) =>
        config.EventMappings.FindIndex(e =>
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) &&
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));
}