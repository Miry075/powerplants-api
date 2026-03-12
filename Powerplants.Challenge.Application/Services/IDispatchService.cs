using System;
using Powerplants.Challenge.Domain.Models;

namespace Powerplants.Challenge.Application.Services;

public interface IDispatchService
{
    IEnumerable<PowerplantProduction> DispatchProduction(ProductionPlanRequest productionPlanRequest);
}

