using Microsoft.Extensions.Logging.Abstractions;
using Powerplants.Challenge.Api.Helpers;
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
            new Powerplant { Name = "p1", Type = "gasfired", Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = "gasfired", Efficiency = 1, PMax = 50, PMin = 0 },
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
        var plants = new[] { new Powerplant { Name = "p1", Type = "gasfired", Efficiency = 1, PMax = 100, PMin = 0 } };
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
            new Powerplant { Name = "p1", Type = "gasfired", Efficiency = 1, PMax = 100, PMin = 0 },
            new Powerplant { Name = "p2", Type = "gasfired", Efficiency = 1, PMax = 50, PMin = 0 }
        };
        var load = 200;
        var result = ProductionHelper.DispatchProductionMethod(load, plants, new FuelsInfo(13.4, 50.8, 20, 0), NullLogger.Instance).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal(100, result[0].production);
        Assert.Equal(50, result[1].production);
    }
}
