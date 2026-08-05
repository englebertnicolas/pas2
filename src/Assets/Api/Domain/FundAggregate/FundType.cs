using System.Text.Json.Serialization;

namespace PAS.Assets.Domain.FundAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FundType {
    Collective,
    Dedicated
}
