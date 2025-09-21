using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/preferred-communication")]
public class PreferredCommunicationController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;

    public PreferredCommunicationController(IMockDatabaseService mockDb)
    {
        _mockDb = mockDb;
    }

    [HttpGet]
    public async Task<IActionResult> GetPreferredCommunication()
    {
        var data = await _mockDb.GetPreferredCommunicationAsync();
        return Ok(data);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SavePreferredCommunication([FromBody] List<PreferredCommunication> data)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, error = "Invalid preferred communication data" });
        
        var success = await _mockDb.SavePreferredCommunicationAsync(data);
        
        if (success)
        {
            return Ok(new { success = true, message = "Preferred Communication saved successfully!" });
        }
        else
        {
            return BadRequest(new { success = false, error = "Failed to save preferred communication" });
        }
    }

}