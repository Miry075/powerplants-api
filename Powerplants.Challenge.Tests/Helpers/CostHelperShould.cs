using System;
using Powerplants.Challenge.Api.Helpers;
using Powerplants.Challenge.Domain.Models;
using Xunit;

namespace Powerplants.Challenge.Helpers.Tests
{
    public class CostHelperShould
    {
        [Fact]
        public void ComputeMarginalCost_GasFired_ReturnsCorrectValue()
        {
            var plant = new Powerplant
            {
                Name = "gas1",
                Type = "gasfired",
                Efficiency = 0.5,
                PMax = 100,
                PMin = 0
            };
            var fuels = new FuelsInfo(13.4, 50, 0, 0);
            var cost = CostHelper.ComputeMarginalCost(plant, fuels);
            Assert.Equal(13.4 / plant.Efficiency, cost);
        }

        [Fact]
        public void ComputeMarginalCost_Turbojet_ReturnsCorrectValue()
        {
            var plant = new Powerplant
            {
                Name = "jet1",
                Type = "turbojet",
                Efficiency = 0.4,
                PMax = 100,
                PMin = 0
            };
            var fuels = new FuelsInfo(20, 80, 0, 0);
            var cost = CostHelper.ComputeMarginalCost(plant, fuels);
            Assert.Equal(80 / plant.Efficiency, cost);
        }

        [Fact]
        public void ComputeMarginalCost_WindTurbine_ReturnsZero()
        {
            var plant = new Powerplant
            {
                Name = "wind1",
                Type = "windturbine",
                Efficiency = 1,
                PMax = 100,
                PMin = 0
            };
            var fuels = new FuelsInfo(0, 0, 0, 50);
            var cost = CostHelper.ComputeMarginalCost(plant, fuels);
            Assert.Equal(0, cost);
        }

        [Fact]
        public void ComputeMarginalCost_UnknownType_Throws()
        {
            var plant = new Powerplant
            {
                Name = "x",
                Type = "unknown",
                Efficiency = 1,
                PMax = 1,
                PMin = 1
            };
            var fuels = new FuelsInfo(0, 0, 0, 0);
            Assert.Throws<ArgumentException>(() => CostHelper.ComputeMarginalCost(plant, fuels));
        }
    }
}
