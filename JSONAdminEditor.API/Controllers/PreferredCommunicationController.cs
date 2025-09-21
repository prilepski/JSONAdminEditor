using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using System.Text.Json;

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
        try
        {
            var data = await _mockDb.GetGlobalDataAsync("preferred-communication");
            var listData = data?["data"] as List<Dictionary<string, object>> ?? new List<Dictionary<string, object>>();
            return Ok(listData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving preferred communication" });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SavePreferredCommunication([FromBody] List<PreferredCommunication> data)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, error = "Invalid preferred communication data" });
            
            var success = await _mockDb.SaveGlobalDataAsync("preferred-communication", new Dictionary<string, object> { ["data"] = data });
            success = true; // Mock always succeeds
            
            if (success)
            {
                return Ok(new { success = true, message = "Preferred Communication saved successfully!" });
            }
            else
            {
                return BadRequest(new { success = false, error = "Failed to save preferred communication" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving preferred communication" });
        }
    }
    
    private static object GetJsonElementValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.TryGetInt32(out var intVal) ? intVal : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
}