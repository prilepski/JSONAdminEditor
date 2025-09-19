using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
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
            return Ok(data ?? new List<Dictionary<string, object>>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving preferred communication" });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> SavePreferredCommunication([FromBody] JsonElement requestData)
    {
        try
        {
            var dataProperty = requestData.GetProperty("data");
            var tableData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(dataProperty.GetRawText());
            var convertedData = tableData?.Select(row => 
                row.ToDictionary(kvp => kvp.Key, kvp => GetJsonElementValue(kvp.Value))
            ).ToList();
            
            var success = await _mockDb.SaveGlobalDataAsync("preferred-communication", new Dictionary<string, object> { ["data"] = convertedData });
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