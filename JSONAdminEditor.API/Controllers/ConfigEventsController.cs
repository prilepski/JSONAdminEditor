using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using System.Text.Json;
using JSONAdminEditor.Application.Models.Configuration;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/config/events")]
[Produces("application/json")]
public class ConfigEventsController : ControllerBase
{
    private readonly IFileContentService _fileService;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigEventsController(IFileContentService fileService)
    {
        _fileService = fileService;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
    }

    private async Task<NotificationMapping> GetConfigAsync()
    {
        var content = await _fileService.ReadFileAsync("data/notifications.json");
        return string.IsNullOrEmpty(content) 
            ? new NotificationMapping() 
            : JsonSerializer.Deserialize<NotificationMapping>(content, _jsonOptions);
    }

    private async Task SaveConfigAsync(NotificationMapping config)
    {
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        await _fileService.WriteFileAsync("data/notifications.json", json);
    }

    private EventMapping? FindEventMapping(NotificationMapping config, string eventName, string orderType)
    {
        return config.EventMappings.FirstOrDefault(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<EventMapping>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<EventMapping>>> GetEvents()
    {
        var config = await GetConfigAsync();
        return Ok(config.EventMappings);
    }

    [HttpGet("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(200, Type = typeof(EventMapping))]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<EventMapping>> GetEvent(string eventName, string orderType)
    {
        var config = await GetConfigAsync();
        var eventMapping = FindEventMapping(config, eventName, orderType);
        
        if (eventMapping == null)
            return NotFound(new { error = "Event mapping not found" });
        
        return Ok(eventMapping);
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

        var config = await GetConfigAsync();
        var existingIndex = config.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        eventMapping.Event = eventName;
        eventMapping.OrderType = orderType;

        if (existingIndex >= 0)
            config.EventMappings[existingIndex] = eventMapping;
        else
            config.EventMappings.Add(eventMapping);

        await SaveConfigAsync(config);
        return NoContent();
    }

    [HttpDelete("{eventName:minlength(1)}/order-types/{orderType:minlength(1)}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteEvent(string eventName, string orderType)
    {
        var config = await GetConfigAsync();
        var existingIndex = config.EventMappings.FindIndex(e => 
            e.Event.Equals(eventName, StringComparison.OrdinalIgnoreCase) && 
            e.OrderType.Equals(orderType, StringComparison.OrdinalIgnoreCase));

        if (existingIndex < 0)
            return NotFound("Event mapping not found");

        config.EventMappings.RemoveAt(existingIndex);
        await SaveConfigAsync(config);
        return NoContent();
    }
}