using Powerplants.Challenge.Domain.Enums;

namespace Powerplants.Challenge.Domain.Models;

public class Powerplant
{
    public required string Name { get; set; }
    public PowerplantType Type { get; set; }
    public double Efficiency { get; set; }
    public double PMin { get; set; }
    public double PMax { get; set; }

    public double ComputeMarginalCost(FuelsInfo fuelsInfo)
    {
        ArgumentNullException.ThrowIfNull(fuelsInfo);

        return Type switch
        {
            PowerplantType.GasFired => fuelsInfo.Gas / Efficiency,
            PowerplantType.TurboJet => fuelsInfo.Kerosine / Efficiency,
            PowerplantType.WindTurbine => 0,
            _ => throw new ArgumentException($"The given fuel type is unknown : {Type}")
        };
    }
}
