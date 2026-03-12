using Microsoft.AspNetCore.Mvc;
using Powerplants.Challenge.Application.Services;
using Powerplants.Challenge.Domain.Models;


namespace Powerplants.Challenge.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductionPlanController : Controller
{
    private readonly ILogger _logger;
    private IDispatchService _dispatchService;

    public ProductionPlanController(ILogger<ProductionPlanController> logger,
        IDispatchService dispatchService)
    {
        _logger = logger;
        _dispatchService = dispatchService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(ProductionPlanRequest productionPlanRequest)
    {
        ArgumentNullException.ThrowIfNull(productionPlanRequest);
        _logger.LogInformation("Received production plan request with load containing {PowerplantCount} powerplants",
            productionPlanRequest.Powerplants.Count);
        try
        {
            var res = _dispatchService.DispatchProduction(productionPlanRequest);
            return Ok(new ProductionPlanResponse(res));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "An argument was null. Message: {Message}", ex.Message);
            return BadRequest(ex);
        }
    }
}
