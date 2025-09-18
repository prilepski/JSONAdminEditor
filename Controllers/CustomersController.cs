using Microsoft.AspNetCore.Mvc;
using JSONAdminEditor.Services;

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
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{customerId}/events")]
    public async Task<IActionResult> GetCustomerEvents(string customerId)
    {
        try
        {
            var data = await _storageService.GetCustomerDataAsync(customerId + "_events");
            return Ok(data ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("{customerId}/events")]
    public async Task<IActionResult> SaveCustomerEvents(string customerId, [FromBody] Dictionary<string, object> data)
    {
        try
        {
            var success = await _storageService.SaveCustomerDataAsync(customerId + "_events", data);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    [HttpGet("{customerId}/settings")]
    public async Task<IActionResult> GetCustomerSettings(string customerId)
    {
        try
        {
            var data = await _storageService.GetCustomerDataAsync(customerId + "_settings");
            return Ok(data ?? new Dictionary<string, object>());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("{customerId}/settings")]
    public async Task<IActionResult> SaveCustomerSettings(string customerId, [FromBody] Dictionary<string, object> data)
    {
        try
        {
            var success = await _storageService.SaveCustomerDataAsync(customerId + "_settings", data);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}