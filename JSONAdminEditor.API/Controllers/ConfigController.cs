using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models.Structure;

namespace JSONAdminEditor.Controllers;

/// <summary>
/// Manages default notification configuration settings
/// </summary>
[ApiController]
[Route("api/config")]
[Produces("application/json")]
public class ConfigController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public ConfigController(IMockDatabaseService mockDb)
    {
        _mockDb = mockDb;
    }

    /// <summary>
    /// Gets the complete default notification configuration
    /// </summary>
    /// <returns>The default notification mapping configuration</returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(NotificationMapping))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<NotificationMapping>> GetConfig()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config);
    }

    /// <summary>
    /// Updates the complete default notification configuration
    /// </summary>
    /// <param name="config">The notification configuration to update</param>
    /// <returns>Success or error response</returns>
    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateConfig([FromBody] NotificationMapping config)
    {
        if (config == null)
            return BadRequest("Configuration data is required");

        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid configuration data");

        var success = await _mockDb.SaveNotificationMappingAsync(config);
        return success ? NoContent() : BadRequest("Failed to update configuration");
    }

    /// <summary>
    /// Gets the preferred communication settings
    /// </summary>
    /// <returns>List of preferred communication configurations</returns>
    [HttpGet("preferred-communication")]
    [ProducesResponseType(200, Type = typeof(List<PreferredCommunication>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<PreferredCommunication>>> GetPreferredCommunication()
    {
        var data = await _mockDb.GetPreferredCommunicationAsync();
        return Ok(data);
    }

    /// <summary>
    /// Updates the preferred communication settings
    /// </summary>
    /// <param name="data">The preferred communication configurations to update</param>
    /// <returns>Success or error response</returns>
    [HttpPut("preferred-communication")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdatePreferredCommunication([FromBody] List<PreferredCommunication> data)
    {
        if (data == null)
            return BadRequest("Preferred communication data is required");

        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid preferred communication data");

        var success = await _mockDb.SavePreferredCommunicationAsync(data);
        return success ? NoContent() : BadRequest("Failed to update preferred communication");
    }

    /// <summary>
    /// Gets the content variables as key-value pairs
    /// </summary>
    /// <returns>List of content variable key-value pairs</returns>
    [HttpGet("content-variables")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, string>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, string>>> GetContentVariables()
    {
        var data = await _mockDb.GetContentVariablesAsync();
        return Ok(data);
    }

    /// <summary>
    /// Updates the content variables
    /// </summary>
    /// <param name="contentVariables">The content variables dictionary to update</param>
    /// <returns>Success or error response</returns>
    [HttpPut("content-variables")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateContentVariables([FromBody] Dictionary<string, string> contentVariables)
    {
        if (contentVariables == null)
            return BadRequest("Content variables data is required");

        var success = await _mockDb.SaveContentVariablesAsync(contentVariables);
        return success ? NoContent() : BadRequest("Failed to update content variables");
    }

    /// <summary>
    /// Gets the opt-out configuration
    /// </summary>
    /// <returns>The opt-out settings</returns>
    [HttpGet("opt-out")]
    [ProducesResponseType(200, Type = typeof(OptOut))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<OptOut>> GetOptOut()
    {
        var data = await _mockDb.GetOptOutAsync();
        return Ok(data);
    }

    /// <summary>
    /// Updates the opt-out configuration
    /// </summary>
    /// <param name="optOut">The opt-out settings to update</param>
    /// <returns>Success or error response</returns>
    [HttpPut("opt-out")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateOptOut([FromBody] OptOut optOut)
    {
        if (optOut == null)
            return BadRequest("Opt-out data is required");

        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid opt-out data");

        var success = await _mockDb.SaveOptOutAsync(optOut);
        return success ? NoContent() : BadRequest("Failed to update opt-out settings");
    }

    /// <summary>
    /// Gets the after-hours configuration
    /// </summary>
    /// <returns>The after-hours settings</returns>
    [HttpGet("after-hours")]
    [ProducesResponseType(200, Type = typeof(AfterHours))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AfterHours?>> GetAfterHours()
    {
        var data = await _mockDb.GetAfterHoursAsync();
        return Ok(data);
    }

    /// <summary>
    /// Updates the after-hours configuration
    /// </summary>
    /// <param name="afterHours">The after-hours settings to update</param>
    /// <returns>Success or error response</returns>
    [HttpPut("after-hours")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateAfterHours([FromBody] AfterHours afterHours)
    {
        if (afterHours == null)
            return BadRequest("After hours data is required");

        if (!ModelState.IsValid)
            return UnprocessableEntity("Invalid after hours data");

        var success = await _mockDb.SaveAfterHoursAsync(afterHours);
        return success ? NoContent() : BadRequest("Failed to update after hours settings");
    }

    /// <summary>
    /// Gets the from email address
    /// </summary>
    /// <returns>The from email address</returns>
    [HttpGet("from-email")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<string>> GetFromEmail()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config.FromEmail ?? "");
    }

    /// <summary>
    /// Updates the from email address
    /// </summary>
    /// <param name="fromEmail">The from email address to set</param>
    /// <returns>Success or error response</returns>
    [HttpPut("from-email")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateFromEmail([FromBody] string fromEmail)
    {
        if (string.IsNullOrEmpty(fromEmail))
            return BadRequest("From email is required");

        var config = await _mockDb.GetNotificationMappingAsync();
        config.FromEmail = fromEmail;
        var success = await _mockDb.SaveNotificationMappingAsync(config);
        return success ? NoContent() : BadRequest("Failed to update from email");
    }

    /// <summary>
    /// Gets the agents configuration
    /// </summary>
    /// <returns>Dictionary of agent names and their enabled status</returns>
    [HttpGet("agents")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, bool>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, bool>>> GetAgents()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config.Agents);
    }

    /// <summary>
    /// Updates the agents configuration
    /// </summary>
    /// <param name="agents">Dictionary of agent names and their enabled status</param>
    /// <returns>Success or error response</returns>
    [HttpPut("agents")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateAgents([FromBody] Dictionary<string, bool> agents)
    {
        if (agents == null)
            return BadRequest("Agents data is required");

        var config = await _mockDb.GetNotificationMappingAsync();
        config.Agents = agents;
        var success = await _mockDb.SaveNotificationMappingAsync(config);
        return success ? NoContent() : BadRequest("Failed to update agents");
    }

    /// <summary>
    /// Gets the content variables overrides configuration
    /// </summary>
    /// <returns>Nested dictionary of content variable overrides</returns>
    [HttpGet("content-variables-overrides")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, Dictionary<string, Dictionary<string, string>>>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> GetContentVariablesOverrides()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config.ContentVariablesOverrides);
    }

    /// <summary>
    /// Updates the content variables overrides configuration
    /// </summary>
    /// <param name="overrides">Nested dictionary of content variable overrides</param>
    /// <returns>Success or error response</returns>
    [HttpPut("content-variables-overrides")]
    [Consumes("application/json")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateContentVariablesOverrides([FromBody] Dictionary<string, Dictionary<string, Dictionary<string, string>>> overrides)
    {
        if (overrides == null)
            return BadRequest("Content variables overrides data is required");

        var config = await _mockDb.GetNotificationMappingAsync();
        config.ContentVariablesOverrides = overrides;
        var success = await _mockDb.SaveNotificationMappingAsync(config);
        return success ? NoContent() : BadRequest("Failed to update content variables overrides");
    }
}