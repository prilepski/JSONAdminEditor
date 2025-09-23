using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using System.Text.Json;
using JSONAdminEditor.Application.Models.Configuration;

namespace JSONAdminEditor.Controllers;

/// <summary>
/// Manages default notification configuration settings
/// </summary>
[ApiController]
[Route("api/config")]
[Produces("application/json")]
public class ConfigController : ControllerBase
{
    private readonly IFileContentService _fileService;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigController(IFileContentService fileService)
    {
        _fileService = fileService;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };
    }

    private async Task<NotificationMapping> GetConfigAsync()
    {
        var content = await _fileService.ReadFileAsync("data/notifications.json");
        return string.IsNullOrEmpty(content) ? new NotificationMapping() : JsonSerializer.Deserialize<NotificationMapping>(content, _jsonOptions);
    }

    private async Task SaveConfigAsync(NotificationMapping config)
    {
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        await _fileService.WriteFileAsync("data/notifications.json", json);
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
        var config = await GetConfigAsync();
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

        await SaveConfigAsync(config);
        return NoContent();
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
        var config = await GetConfigAsync();
        return Ok(config.PreferredCommunication);
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

        var config = await GetConfigAsync();
        config.PreferredCommunication = data;
        await SaveConfigAsync(config);
        return NoContent();
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
        var config = await GetConfigAsync();
        return Ok(config.ContentVariables);
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

        var config = await GetConfigAsync();
        config.ContentVariables = contentVariables;
        await SaveConfigAsync(config);
        return NoContent();
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
        var config = await GetConfigAsync();
        return Ok(config.OptOut);
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

        var config = await GetConfigAsync();
        config.OptOut = optOut;
        await SaveConfigAsync(config);
        return NoContent();
    }

    /// <summary>
    /// Gets the after-hours configuration
    /// </summary>
    /// <returns>The after-hours settings</returns>
    [HttpGet("after-hours")]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AfterHours?>> GetAfterHours()
    {
        var config = await GetConfigAsync();
        return Ok(config.AfterHours);
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

        var config = await GetConfigAsync();
        config.AfterHours = afterHours;
        await SaveConfigAsync(config);
        return NoContent();
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
        var config = await GetConfigAsync();
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

        var config = await GetConfigAsync();
        config.FromEmail = fromEmail;
        await SaveConfigAsync(config);
        return NoContent();
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
        var config = await GetConfigAsync();
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

        var config = await GetConfigAsync();
        config.Agents = agents;
        await SaveConfigAsync(config);
        return NoContent();
    }
}