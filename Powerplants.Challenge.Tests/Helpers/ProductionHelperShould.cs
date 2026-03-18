using Microsoft.Extensions.Logging.Abstractions;
using Powerplants.Challenge.Api.Helpers;
using Powerplants.Challenge.Domain.Enums;
using Powerplants.Challenge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Powerplants.Challenge.Helpers.Tests;

public class ProductionHelperShould
{

    [Fact]
    public void DispatchProductionMethod_AssignsCorrectlyForExactLoad()
    {
        var plants = new[]
        {
            new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 50, PMin = 0 },
        };
        var load = 120;
        var result = ProductionHelper.DispatchProductionMethod(load, plants, new FuelsInfo(13.4, 50.8, 20, 0), NullLogger.Instance).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal(100, result[0].production);
        Assert.Equal(20, result[1].production);
    }

    [Fact]
    public void DispatchProductionMethod_LoadLessThanFirstPmax()
    {
        var plants = new[] { new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 } };
        var load = 30;
        var result = ProductionHelper.DispatchProductionMethod(load, plants, new FuelsInfo(13.4, 50.8, 20, 0), NullLogger.Instance);
        var rp = Assert.Single(result);
        Assert.Equal("p1", rp.powerplantName);
        Assert.Equal(30, rp.production);
    }

    [Fact]
    public void DispatchProductionMethod_LoadExceedsTotalReturnsAllPmax()
    {
        var plants = new[]
        {
            new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 50, PMin = 0 }
        };
        var load = 200;
        var result = ProductionHelper.DispatchProductionMethod(load, plants, new FuelsInfo(13.4, 50.8, 20, 0), NullLogger.Instance).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal(100, result[0].production);
        Assert.Equal(50, result[1].production);
    }

    [Fact]
    public void DispatchProductionMethod_RebalancesAcrossMultiplePlants_WhenShortfallBelowPmin()
    {
        var plants = new[]
        {
            new Powerplant { Name = "p1", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 60 },
            new Powerplant { Name = "p3", Type = PowerplantType.GasFired, Efficiency = 1, PMax = 100, PMin = 60 },
        };

        var load = 130;
        var result = ProductionHelper.DispatchProductionMethod(load, plants, new FuelsInfo(13.4, 50.8, 20, 0), NullLogger.Instance).ToList();

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
