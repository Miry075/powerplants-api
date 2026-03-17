using System;
using Powerplants.Challenge.Domain.Models;

namespace Powerplants.Challenge.Api.Helpers;

public static class ProductionHelper
{
    public static IEnumerable<PowerplantProduction> DispatchProductionMethod(double load,
        IEnumerable<Powerplant> powerplants,
        FuelsInfo fuelsInfo,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(powerplants);
        ArgumentNullException.ThrowIfNull(logger);

        var results = new List<PowerplantProduction>();
        var powerplantsList = powerplants.ToList();

        logger.LogInformation("Dispatch production: get started");
        var dispatchState = new List<(Powerplant plant, double production)>();

        foreach (var powerplant in powerplantsList)
        {
            var effectivePMax = GetEffectivePMax(powerplant, fuelsInfo);
            var effectivePMin = GetEffectivePMin(powerplant);
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
            logger.LogInformation("Dispatch production proceeds with: powerplant {PowerplantName}", powerplant.Name);
        }

        foreach (var (plant, production) in dispatchState)
            results.Add(new PowerplantProduction(plant.Name, production));

        logger.LogInformation("Dispatch production returned values");
        return results;
    }

    private static bool TryRebalanceForShortfall(List<(Powerplant plant, double production)> dispatchState, double shortfall)
    {
        var remainingShortfall = shortfall;

        for (int i = dispatchState.Count - 1; i >= 0 && remainingShortfall > 0; i--)
        {
            var previous = dispatchState[i];
            var previousEffectivePMin = GetEffectivePMin(previous.plant);
            var reducible = Math.Max(0, previous.production - previousEffectivePMin);
            if (reducible <= 0)
            {
                continue;
            }

            var reduction = Math.Min(reducible, remainingShortfall);
            dispatchState[i] = (previous.plant, previous.production - reduction);
            remainingShortfall -= reduction;
        }

        return remainingShortfall <= 0;
    }

    private static double GetEffectivePMin(Powerplant powerplant)
    {
        return powerplant.Type == "windturbine" ? 0 : powerplant.PMin;
    }

    private static double GetEffectivePMax(Powerplant powerplant, FuelsInfo fuelsInfo)
    {
        if (powerplant.Type == "windturbine")
        {
            return powerplant.PMax * fuelsInfo.Wind / 100;
        }

        return powerplant.PMax;
    }
}


