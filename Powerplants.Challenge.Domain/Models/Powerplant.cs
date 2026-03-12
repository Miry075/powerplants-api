namespace Powerplants.Challenge.Domain.Models;

public class Powerplant
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public double Efficiency { get; set; }
    public double PMin { get; set; }
    public double PMax { get; set; }
}
