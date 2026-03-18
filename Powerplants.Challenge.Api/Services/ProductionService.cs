using Powerplants.Challenge.Application.Services;
using Powerplants.Challenge.Domain.Enums;
using Powerplants.Challenge.Domain.Models;

namespace Powerplants.Challenge.Api.Services;

public class ProductionService : IProductionService
{
    private readonly ILogger<ProductionService> _logger;

    public ProductionService(ILogger<ProductionService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<PowerplantProduction> DispatchProduction(double load,
        IEnumerable<Powerplant> powerplants,
        FuelsInfo fuelsInfo)
    {
        ArgumentNullException.ThrowIfNull(powerplants);

        var results = new List<PowerplantProduction>();
        var powerplantsList = powerplants.ToList();

        _logger.LogInformation("Dispatch production: get started");
        var dispatchState = new List<(Powerplant plant, double production)>();

        foreach (var powerplant in powerplantsList)
        {
            var effectivePMax = GetEffectivePMax(powerplant, fuelsInfo);
            var effectivePMin = powerplant.PMin;
            var production = 0d;

            if (load <= 0)
            {
            }
            else if (load >= effectivePMin)
            {
                production = Math.Min(effectivePMax, load);
                load -= production;
            }
            else
            {
                var shortfall = effectivePMin - load;
                var rebalanced = TryRebalanceForShortfall(dispatchState, shortfall);
                if (rebalanced)
                {
                    production = effectivePMin;
                    load = 0;
                }
            }

            dispatchState.Add((powerplant, production));
            _logger.LogInformation("Dispatch production proceeds with: powerplant {PowerplantName}", powerplant.Name);
        }

        foreach (var (plant, production) in dispatchState)
            results.Add(new PowerplantProduction(plant.Name, production));

        _logger.LogInformation("Dispatch production returned values");
        return results;
    }

    private static bool TryRebalanceForShortfall(List<(Powerplant plant, double production)> dispatchState, double shortfall)
    {
        var remainingShortfall = shortfall;

        for (int i = dispatchState.Count - 1; i >= 0 && remainingShortfall > 0; i--)
        {
            var previous = dispatchState[i];
            var reducible = Math.Max(0, previous.production - previous.plant.PMin);
            if (reducible <= 0)
                continue;

            var reduction = Math.Min(reducible, remainingShortfall);
            dispatchState[i] = (previous.plant, previous.production - reduction);
            remainingShortfall -= reduction;
        }

        return remainingShortfall <= 0;
    }

    private static double GetEffectivePMax(Powerplant powerplant, FuelsInfo fuelsInfo)
    {
        if (powerplant.Type == PowerplantType.WindTurbine)
            return powerplant.PMax * fuelsInfo.Wind / 100;

        return powerplant.PMax;
    }
}
