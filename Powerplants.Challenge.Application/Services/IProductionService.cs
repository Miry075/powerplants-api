using Powerplants.Challenge.Domain.Models;

namespace Powerplants.Challenge.Application.Services;

public interface IProductionService
{
    IEnumerable<PowerplantProduction> DispatchProduction(double load,
        IEnumerable<Powerplant> powerplants,
        FuelsInfo fuelsInfo);
}
