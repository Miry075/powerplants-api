using System;
using Powerplants.Challenge.Application.Services;
using Powerplants.Challenge.Domain.Models;


namespace Powerplants.Challenge.Api.Services;

public class DispatchService : IDispatchService
{
    private readonly ILogger _logger;
    private readonly IProductionService _productionService;

    public DispatchService(ILogger<DispatchService> logger, IProductionService productionService)
    {
        _logger = logger;
        _productionService = productionService;
    }

    public IEnumerable<PowerplantProduction> DispatchProduction(ProductionPlanRequest productionPlanRequest)
    {
        try
        {
            _logger.LogInformation("Starting dispatching production for load {Load} with {PowerplantCount} powerplants",
            productionPlanRequest.Load, productionPlanRequest.Powerplants.Count);
            var meritOrderedPowerplants = productionPlanRequest
                .Powerplants
                .OrderBy(x => x.ComputeMarginalCost(productionPlanRequest.Fuels));
            return _productionService.DispatchProduction(productionPlanRequest.Load,
                meritOrderedPowerplants,
                productionPlanRequest.Fuels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while dispatching production. Message: {Message}", ex.Message);
            throw;
        }
    }
}

