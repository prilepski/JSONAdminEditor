using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;
using System.Text.RegularExpressions;

namespace JSONAdminEditor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IStorageService _storageService;

    public CustomersController(IStorageServiceFactory storageServiceFactory)
    {
        _storageService = storageServiceFactory.CreateStorageService();
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
            var customers = await _storageService.SearchCustomersAsync("");
            var customerIds = customers.Select(c => c.CustomerId).ToList();
            return Ok(customerIds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving customers" });
        }
    }

    [HttpGet("{customerId}/events")]
    public async Task<IActionResult> GetCustomerEvents(string customerId)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { error = "Invalid customer ID" });
                
            var data = await _storageService.GetCustomerDataAsync($"{customerId}_events");
            return Ok(data ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving customer events" });
        }
    }

    [HttpPost("{customerId}/events")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCustomerEvents(string customerId, [FromBody] Dictionary<string, object> data)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { success = false, error = "Invalid customer ID" });
                
            var success = await _storageService.SaveCustomerDataAsync($"{customerId}_events", data);
            return Ok(new { success });
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
                
            var data = await _storageService.GetCustomerDataAsync($"{customerId}_settings");
            return Ok(data ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving customer settings" });
        }
    }

    [HttpPost("{customerId}/settings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCustomerSettings(string customerId, [FromBody] Dictionary<string, object> data)
    {
        try
        {
            if (!IsValidCustomerId(customerId))
                return BadRequest(new { success = false, error = "Invalid customer ID" });
                
            var success = await _storageService.SaveCustomerDataAsync($"{customerId}_settings", data);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "An error occurred while saving customer settings" });
        }
    }
}