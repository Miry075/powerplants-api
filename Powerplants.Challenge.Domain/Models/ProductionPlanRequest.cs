using System;
namespace Powerplants.Challenge.Domain.Models;

public record ProductionPlanRequest(double Load, FuelsInfo Fuels, List<Powerplant> Powerplants)
{
}

