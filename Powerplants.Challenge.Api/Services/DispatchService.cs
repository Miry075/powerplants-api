using System;
using Powerplants.Challenge.Api.Helpers;
using Powerplants.Challenge.Application.Services;
using Powerplants.Challenge.Domain.Models;


namespace Powerplants.Challenge.Api.Services;

public class DispatchService : IDispatchService
{
    private readonly ILogger _logger;

    public DispatchService(ILogger<DispatchService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<PowerplantProduction> DispatchProduction(ProductionPlanRequest productionPlanRequest)
    {
        try
        {
            _logger.LogInformation("Starting dispatching production for load {Load} with {PowerplantCount} powerplants",
            productionPlanRequest.Load, productionPlanRequest.Powerplants.Count);
            var meritOrderedPowerplants = productionPlanRequest
                .Powerplants
                .OrderBy(x => CostHelper.ComputeMarginalCost(x, productionPlanRequest.Fuels));
            return ProductionHelper.DispatchProductionMethod(productionPlanRequest.Load,
                meritOrderedPowerplants,
                productionPlanRequest.Fuels,
                _logger);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while dispatching production. Message: {Message}", ex.Message);
            throw;
        }

    }
}

