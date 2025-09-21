using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Controllers;

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

    // Root level endpoints
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(NotificationMapping))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<NotificationMapping>> GetConfig()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config);
    }

    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateConfig([FromBody] NotificationMapping config)
    {
        if (config == null)
            return BadRequest(new { success = false, error = "Configuration data is required" });

        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid configuration data" });

        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "Configuration updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update configuration" });
        }
    }

    [HttpGet("preferred-communication")]
    [ProducesResponseType(200, Type = typeof(List<PreferredCommunication>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<PreferredCommunication>>> GetPreferredCommunication()
    {
        var data = await _mockDb.GetPreferredCommunicationAsync();
        return Ok(data);
    }

    [HttpPut("preferred-communication")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdatePreferredCommunication([FromBody] List<PreferredCommunication> data)
    {
        if (data == null)
            return BadRequest(new { success = false, error = "Preferred communication data is required" });

        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid preferred communication data" });

        var success = await _mockDb.SavePreferredCommunicationAsync(data);

        if (success)
        {
            return Ok(new { success = true, message = "Preferred communication updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update preferred communication" });
        }
    }

    [HttpGet("content-variables")]
    [ProducesResponseType(200, Type = typeof(List<object>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<object>>> GetContentVariables()
    {
        var data = await _mockDb.GetContentVariablesAsync();
        var listData = data.Select(kvp => new { key = kvp.Key, value = kvp.Value }).ToList();
        return Ok(listData);
    }

    [HttpPut("content-variables")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateContentVariables([FromBody] Dictionary<string, string> contentVariables)
    {
        if (contentVariables == null)
            return BadRequest(new { success = false, error = "Content variables data is required" });

        var success = await _mockDb.SaveContentVariablesAsync(contentVariables);

        if (success)
        {
            return Ok(new { success = true, message = "Content variables updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update content variables" });
        }
    }

    [HttpGet("opt-out")]
    [ProducesResponseType(200, Type = typeof(OptOut))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<OptOut>> GetOptOut()
    {
        var data = await _mockDb.GetOptOutAsync();
        return Ok(data);
    }

    [HttpPut("opt-out")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateOptOut([FromBody] OptOut optOut)
    {
        if (optOut == null)
            return BadRequest(new { success = false, error = "Opt-out data is required" });

        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid opt-out data" });

        var success = await _mockDb.SaveOptOutAsync(optOut);

        if (success)
        {
            return Ok(new { success = true, message = "Opt-out settings updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update opt-out settings" });
        }
    }

    [HttpGet("after-hours")]
    [ProducesResponseType(200, Type = typeof(AfterHours))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AfterHours?>> GetAfterHours()
    {
        var data = await _mockDb.GetAfterHoursAsync();
        return Ok(data);
    }

    [HttpPut("after-hours")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateAfterHours([FromBody] AfterHours afterHours)
    {
        if (afterHours == null)
            return BadRequest(new { success = false, error = "After hours data is required" });

        if (!ModelState.IsValid)
            return UnprocessableEntity(new { success = false, error = "Invalid after hours data" });

        var success = await _mockDb.SaveAfterHoursAsync(afterHours);

        if (success)
        {
            return Ok(new { success = true, message = "After hours settings updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update after hours settings" });
        }
    }

    [HttpGet("from-email")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<string>> GetFromEmail()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config.FromEmail ?? "");
    }

    [HttpPut("from-email")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateFromEmail([FromBody] string fromEmail)
    {
        if (string.IsNullOrEmpty(fromEmail))
            return BadRequest(new { success = false, error = "From email is required" });

        var config = await _mockDb.GetNotificationMappingAsync();
        config.FromEmail = fromEmail;
        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "From email updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update from email" });
        }
    }

    [HttpGet("agents")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, bool>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, bool>>> GetAgents()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config.Agents);
    }

    [HttpPut("agents")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateAgents([FromBody] Dictionary<string, bool> agents)
    {
        if (agents == null)
            return BadRequest(new { success = false, error = "Agents data is required" });

        var config = await _mockDb.GetNotificationMappingAsync();
        config.Agents = agents;
        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "Agents updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update agents" });
        }
    }

    [HttpGet("content-variables-overrides")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, Dictionary<string, Dictionary<string, string>>>))]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> GetContentVariablesOverrides()
    {
        var config = await _mockDb.GetNotificationMappingAsync();
        return Ok(config.ContentVariablesOverrides);
    }

    [HttpPut("content-variables-overrides")]
    [Consumes("application/json")]
    [ProducesResponseType(200, Type = typeof(object))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateContentVariablesOverrides([FromBody] Dictionary<string, Dictionary<string, Dictionary<string, string>>> overrides)
    {
        if (overrides == null)
            return BadRequest(new { success = false, error = "Content variables overrides data is required" });

        var config = await _mockDb.GetNotificationMappingAsync();
        config.ContentVariablesOverrides = overrides;
        var success = await _mockDb.SaveNotificationMappingAsync(config);

        if (success)
        {
            return Ok(new { success = true, message = "Content variables overrides updated successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to update content variables overrides" });
        }
    }
}