using System;
namespace Powerplants.Challenge.Domain.Models;

public record ProductionPlanResponse(IEnumerable<PowerplantProduction> PowerplantProduction)
{
}

