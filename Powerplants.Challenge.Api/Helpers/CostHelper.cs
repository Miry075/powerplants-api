using System;
using Powerplants.Challenge.Domain.Models;

namespace Powerplants.Challenge.Api.Helpers;

public static class CostHelper
{

    public static double ComputeMarginalCost(Powerplant powerPlant, FuelsInfo fuelsInfo)
    {
        switch (powerPlant.Type)
        {
            case "gasfired":
                return fuelsInfo.Gas / powerPlant.Efficiency;
            case "turbojet":
                return fuelsInfo.Kerosine / powerPlant.Efficiency;
            case "windturbine":
                return 0;
        }
        throw new ArgumentException($"The given fuel type is unknown : {powerPlant.Type}");
    }
}


