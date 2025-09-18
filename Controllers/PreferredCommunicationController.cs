using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/preferred-communication")]
public class PreferredCommunicationController : ControllerBase
{
    private readonly IJsonFileService _jsonFileService;

    public PreferredCommunicationController(IJsonFileService jsonFileService)
    {
        _jsonFileService = jsonFileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPreferredCommunication()
    {
        try
        {
            var jsonData = await _jsonFileService.LoadJsonFileAsync("data/preferred-communication.json");
            return Ok(jsonData.TableData ?? new List<Dictionary<string, object>>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SavePreferredCommunication([FromBody] List<Dictionary<string, object>> data)
    {
        try
        {
            var success = await _jsonFileService.SaveJsonFileAsync("data/preferred-communication.json", data);
            
            if (success)
            {
                return Ok(new { success = true, message = "Preferred Communication saved successfully!" });
            }
            else
            {
                return Ok(new { success = false, error = "Failed to save preferred communication" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}