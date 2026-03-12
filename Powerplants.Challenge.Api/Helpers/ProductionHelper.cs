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
        var results = new List<PowerplantProduction>();
        var powerplantsList = powerplants.ToList();

        logger.LogInformation("Dispatch production: get started");
        foreach (var powerplant in powerplantsList)
        {
            var effectivePMax = GetEffectivePMax(powerplant, fuelsInfo);
            var production = 0d;

            if (load > 0)
            {
                production = Math.Min(effectivePMax, load);
                load -= production;
            }

            results.Add(new PowerplantProduction(powerplant.Name, production));
            logger.LogInformation("Dispatch production proceeds with: powerplant {PowerplantName}", powerplant.Name);
        }

        logger.LogInformation("Dispatch production returned values");
        return results;
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


