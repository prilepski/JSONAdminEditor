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
            // Implementation would depend on your customer events storage
            // For now, return empty object
            return Ok(new { });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("{customerId}/events")]
    public async Task<IActionResult> SaveCustomerEvents(string customerId, [FromBody] dynamic data)
    {
        try
        {
            // Implementation would depend on your customer events storage
            return Ok(new { success = true });
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
            // Implementation would depend on your customer settings storage
            return Ok(new { });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("{customerId}/settings")]
    public async Task<IActionResult> SaveCustomerSettings(string customerId, [FromBody] dynamic data)
    {
        try
        {
            // Implementation would depend on your customer settings storage
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}