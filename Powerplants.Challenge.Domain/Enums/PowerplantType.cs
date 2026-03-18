using System.Text.Json.Serialization;

namespace Powerplants.Challenge.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<PowerplantType>))]
public enum PowerplantType
{
    [JsonStringEnumMemberName("gasfired")]
    GasFired,

    [JsonStringEnumMemberName("turbojet")]
    TurboJet,

    [JsonStringEnumMemberName("windturbine")]
    WindTurbine
}
