using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Powerplants.Challenge.Api.Services;
using Powerplants.Challenge.Domain.Enums;
using Powerplants.Challenge.Domain.Models;
using Xunit;

namespace Powerplants.Challenge.Services.Tests;

public class DispatchServiceShould
{
    [Fact]
    public void DispatchProduction_WithGivenPayload_ReturnExpectedPlan()
    {
        var request = new ProductionPlanRequest(
            480,
            new FuelsInfo(13.4, 50.8, 20, 60),
            new()
            {
                new Powerplant { Name = "gasfiredbig1", Type = PowerplantType.GasFired, Efficiency = 0.53, PMin = 100, PMax = 460 },
                new Powerplant { Name = "gasfiredbig2", Type = PowerplantType.GasFired, Efficiency = 0.53, PMin = 100, PMax = 460 },
                new Powerplant { Name = "gasfiredsomewhatsmaller", Type = PowerplantType.GasFired, Efficiency = 0.37, PMin = 40, PMax = 210 },
                new Powerplant { Name = "tj1", Type = PowerplantType.TurboJet, Efficiency = 0.3, PMin = 0, PMax = 16 },
                new Powerplant { Name = "windpark1", Type = PowerplantType.WindTurbine, Efficiency = 1, PMin = 0, PMax = 150 },
                new Powerplant { Name = "windpark2", Type = PowerplantType.WindTurbine, Efficiency = 1, PMin = 0, PMax = 36 },
            });

        var service = new DispatchService(NullLogger<DispatchService>.Instance);

        var result = service.DispatchProduction(request).ToList();

        Assert.Equal(6, result.Count);

        Assert.Equal("windpark1", result[0].powerplantName);
        Assert.Equal(90, result[0].production);

        Assert.Equal("windpark2", result[1].powerplantName);
        Assert.Equal(21.6, result[1].production);

        Assert.Equal("gasfiredbig1", result[2].powerplantName);
        Assert.Equal(368.4, result[2].production);

        Assert.Equal("gasfiredbig2", result[3].powerplantName);
        Assert.Equal(0, result[3].production);

        Assert.Equal("gasfiredsomewhatsmaller", result[4].powerplantName);
        Assert.Equal(0, result[4].production);

        Assert.Equal("tj1", result[5].powerplantName);
        Assert.Equal(0, result[5].production);

        Assert.Equal(480, result.Sum(x => x.production));
    }

    [Fact]
    public void DispatchProduction_WithZeroWindPayload_ReturnExpectedPlan()
    {
        var request = new ProductionPlanRequest(
            480,
            new FuelsInfo(13.4, 50.8, 20, 0),
            new()
            {
                new Powerplant { Name = "gasfiredbig1", Type = PowerplantType.GasFired, Efficiency = 0.53, PMin = 100, PMax = 460 },
                new Powerplant { Name = "gasfiredbig2", Type = PowerplantType.GasFired, Efficiency = 0.53, PMin = 100, PMax = 460 },
                new Powerplant { Name = "gasfiredsomewhatsmaller", Type = PowerplantType.GasFired, Efficiency = 0.37, PMin = 40, PMax = 210 },
                new Powerplant { Name = "tj1", Type = PowerplantType.TurboJet, Efficiency = 0.3, PMin = 0, PMax = 16 },
                new Powerplant { Name = "windpark1", Type = PowerplantType.WindTurbine, Efficiency = 1, PMin = 0, PMax = 150 },
                new Powerplant { Name = "windpark2", Type = PowerplantType.WindTurbine, Efficiency = 1, PMin = 0, PMax = 36 },
            });

        var service = new DispatchService(NullLogger<DispatchService>.Instance);

        var result = service.DispatchProduction(request).ToList();

        Assert.Equal(6, result.Count);

        Assert.Equal("windpark1", result[0].powerplantName);
        Assert.Equal(0, result[0].production);

        Assert.Equal("windpark2", result[1].powerplantName);
        Assert.Equal(0, result[1].production);

        Assert.Equal("gasfiredbig1", result[2].powerplantName);
        Assert.Equal(380, result[2].production);

        Assert.Equal("gasfiredbig2", result[3].powerplantName);
        Assert.Equal(100, result[3].production);

        Assert.Equal("gasfiredsomewhatsmaller", result[4].powerplantName);
        Assert.Equal(0, result[4].production);

        Assert.Equal("tj1", result[5].powerplantName);
        Assert.Equal(0, result[5].production);

        Assert.Equal(480, result.Sum(x => x.production));
    }

    [Fact]
    public void DispatchProduction_WithHighLoadPayload_ReturnExpectedPlan()
    {
        var request = new ProductionPlanRequest(
            910,
            new FuelsInfo(13.4, 50.8, 20, 60),
            new()
            {
                new Powerplant { Name = "gasfiredbig1", Type = PowerplantType.GasFired, Efficiency = 0.53, PMin = 100, PMax = 460 },
                new Powerplant { Name = "gasfiredbig2", Type = PowerplantType.GasFired, Efficiency = 0.53, PMin = 100, PMax = 460 },
                new Powerplant { Name = "gasfiredsomewhatsmaller", Type = PowerplantType.GasFired, Efficiency = 0.37, PMin = 40, PMax = 210 },
                new Powerplant { Name = "tj1", Type = PowerplantType.TurboJet, Efficiency = 0.3, PMin = 0, PMax = 16 },
                new Powerplant { Name = "windpark1", Type = PowerplantType.WindTurbine, Efficiency = 1, PMin = 0, PMax = 150 },
                new Powerplant { Name = "windpark2", Type = PowerplantType.WindTurbine, Efficiency = 1, PMin = 0, PMax = 36 },
            });

        var service = new DispatchService(NullLogger<DispatchService>.Instance);

        var result = service.DispatchProduction(request).ToList();

        Assert.Equal(6, result.Count);

        Assert.Equal("windpark1", result[0].powerplantName);
        Assert.Equal(90, result[0].production);

        Assert.Equal("windpark2", result[1].powerplantName);
        Assert.Equal(21.6, result[1].production);

        Assert.Equal("gasfiredbig1", result[2].powerplantName);
        Assert.Equal(460, result[2].production);

        Assert.Equal("gasfiredbig2", result[3].powerplantName);
        Assert.Equal(338.4, result[3].production);

        Assert.Equal("gasfiredsomewhatsmaller", result[4].powerplantName);
        Assert.Equal(0, result[4].production);

        Assert.Equal("tj1", result[5].powerplantName);
        Assert.Equal(0, result[5].production);

        Assert.Equal(910, result.Sum(x => x.production));
    }
}
