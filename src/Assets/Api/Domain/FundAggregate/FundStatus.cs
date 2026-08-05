using System.Text.Json.Serialization;

namespace PAS.Assets.Domain.FundAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FundStatus {
    Active,
    Suspended,
    Closed
}
