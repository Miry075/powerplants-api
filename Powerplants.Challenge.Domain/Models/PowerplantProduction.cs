using System;
using System.Text.Json.Serialization;
namespace Powerplants.Challenge.Domain.Models;

public record PowerplantProduction(
	[property: JsonPropertyName("name")] string powerplantName,
	[property: JsonPropertyName("p")] double production)
{

}

