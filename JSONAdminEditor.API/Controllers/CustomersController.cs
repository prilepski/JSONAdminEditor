using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using JSONAdminEditor.Application.Models;
using JSONAdminEditor.Models;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMockDatabaseService _mockDb;
    private readonly IDataMigrationService _migrationService;

    public CustomersController(IMockDatabaseService mockDb, IDataMigrationService migrationService)
    {
        _mockDb = mockDb;
        _migrationService = migrationService;
    }

    private static bool IsValidCustomerId(string customerId)
    {
        return !string.IsNullOrWhiteSpace(customerId) && 
               Regex.IsMatch(customerId, @"^[a-zA-Z0-9_-]+$") && 
               customerId.Length <= 50;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        try
        {
            var customers = await _mockDb.SearchCustomersAsync("");
            var customerIds = customers.Select(c => c.TryGetValue("customerId", out var id) ? id?.ToString() : "").Where(id => !string.IsNullOrEmpty(id)).ToList();
            return Ok(customerIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving customers" });
        }
    }

    [HttpGet("{customerId}/events")]
    public async Task<IActionResult> GetCustomerEvents(string customerId, [FromQuery] string? orderType = null)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { error = "Invalid customer ID" });
                
            var data = await _mockDb.GetCustomerAsync(customerId);
            return Ok(data ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving customer events" });
        }
    }

    [HttpPost("{customerId}/events")]
    public async Task<IActionResult> SaveCustomerEvents(string customerId, [FromBody] Dictionary<string, object> eventData)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { success = false, error = "Invalid customer ID" });
            
            // Get existing customer data
            var customerData = await _mockDb.GetCustomerAsync(customerId) ?? new Dictionary<string, object>();
            
            // Get existing events or create new list
            var events = new List<Dictionary<string, object>>();
            if (customerData.TryGetValue("Events", out var eventsObj) && eventsObj is List<Dictionary<string, object>> eventsList)
            {
                events = eventsList;
            }
            
            // Find and update existing event or add new one
            var eventName = eventData.TryGetValue("Event", out var nameObj) ? nameObj?.ToString() : "";
            var orderType = eventData.TryGetValue("OrderType", out var typeObj) ? typeObj?.ToString() : "";
            
            var existingIndex = events.FindIndex(e => 
                e.TryGetValue("Event", out var existingEvent) && 
                existingEvent?.ToString() == eventName &&
                e.TryGetValue("OrderType", out var existingOrderType) &&
                existingOrderType?.ToString() == orderType);
            
            if (existingIndex >= 0)
            {
                events[existingIndex] = eventData;
            }
            else
            {
                events.Add(eventData);
            }
            
            // Update customer data with new events
            customerData["Events"] = events;
            
            var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
            
            if (success)
            {
                // Return the updated customer data
                return Ok(new { success = true, data = customerData });
            }
            else
            {
                return Ok(new { success = false, error = "Failed to save customer events" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving customer events" });
        }
    }

    [HttpGet("{customerId}/settings")]
    public async Task<IActionResult> GetCustomerSettings(string customerId)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { error = "Invalid customer ID" });
                
            var data = await _mockDb.GetCustomerAsync(customerId);
            return Ok(data ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving customer settings" });
        }
    }

    [HttpPost("{customerId}/settings")]
    public async Task<IActionResult> SaveCustomerSettings(string customerId, [FromBody] Dictionary<string, object> settingsData)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { success = false, error = "Invalid customer ID" });
            
            // Get existing customer data
            var customerData = await _mockDb.GetCustomerAsync(customerId) ?? new Dictionary<string, object>();
            
            // Merge settings data into customer data
            foreach (var kvp in settingsData)
            {
                customerData[kvp.Key] = kvp.Value;
            }
            
            var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
            
            if (success)
            {
                // Return the updated customer data
                return Ok(new { success = true, data = customerData });
            }
            else
            {
                return Ok(new { success = false, error = "Failed to save customer settings" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving customer settings" });
        }
    }

    [HttpGet("{customerId}/afterhours")]
    public async Task<IActionResult> GetCustomerAfterHours(string customerId)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { error = "Invalid customer ID" });
                
            var data = await _mockDb.GetCustomerAsync(customerId);
            if (data?.TryGetValue("AfterHours", out var ah) == true && ah is Dictionary<string, object> ahDict)
            {
                var afterHours = _migrationService.MigrateToModel<AfterHours>(ahDict);
                return Ok(afterHours);
            }
            return Ok(null);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving after hours settings" });
        }
    }

    [HttpPost("{customerId}/afterhours")]
    public async Task<IActionResult> SaveCustomerAfterHours(string customerId, [FromBody] AfterHours afterHours)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { success = false, error = "Invalid customer ID" });
            
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, error = "Invalid after hours data" });
            
            var customerData = await _mockDb.GetCustomerAsync(customerId) ?? new Dictionary<string, object>();
            customerData["AfterHours"] = _migrationService.MigrateFromModel(afterHours);
            
            var success = await _mockDb.SaveCustomerAsync(customerId, customerData);
            
            return Ok(success ? ApiResponse<AfterHours>.SuccessResult(afterHours, "After hours settings saved successfully!") : ApiResponse<AfterHours>.ErrorResult("Failed to save after hours settings"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving after hours settings" });
        }
    }
}