using Microsoft.Extensions.Logging.Abstractions;
using Powerplants.Challenge.Api.Services;
using Powerplants.Challenge.Domain.Enums;
using Powerplants.Challenge.Domain.Models;
using System.Linq;
using Xunit;

namespace Powerplants.Challenge.Services.Tests;

public class ProductionServiceShould
{
    private readonly ProductionService _service = new(NullLogger<ProductionService>.Instance);

    [Fact]
    public void DispatchProduction_AssignsCorrectlyForExactLoad()
    {
        var plants = new[]
        {
            new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 50, PMin = 0 },
        };

        var result = _service.DispatchProduction(120, plants, new FuelsInfo(13.4, 50.8, 20, 0)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(100, result[0].production);
        Assert.Equal(20, result[1].production);
    }

    [Fact]
    public void DispatchProduction_LoadLessThanFirstPmax()
    {
        var plants = new[]
        {
            new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 }
        };

        var rp = Assert.Single(_service.DispatchProduction(30, plants, new FuelsInfo(13.4, 50.8, 20, 0)));

        Assert.Equal("p1", rp.powerplantName);
        Assert.Equal(30, rp.production);
    }

    [Fact]
    public void DispatchProduction_LoadExceedsTotalReturnsAllPmax()
    {
        var plants = new[]
        {
            new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 50, PMin = 0 }
        };

        var result = _service.DispatchProduction(200, plants, new FuelsInfo(13.4, 50.8, 20, 0)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(100, result[0].production);
        Assert.Equal(50, result[1].production);
    }

    [Fact]
    public void DispatchProduction_RebalancesAcrossMultiplePlants_WhenShortfallBelowPmin()
    {
        var plants = new[]
        {
            new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 60 },
            new Powerplant { Name = "p3", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 60 },
        };

        var result = _service.DispatchProduction(130, plants, new FuelsInfo(13.4, 50.8, 20, 0)).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("p1", result[0].powerplantName);
        Assert.Equal(70, result[0].production);
        Assert.Equal("p2", result[1].powerplantName);
        Assert.Equal(60, result[1].production);
        Assert.Equal("p3", result[2].powerplantName);
        Assert.Equal(0, result[2].production);
        Assert.Equal(130, result.Sum(x => x.production));
    }
}
