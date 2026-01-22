using System.Text.Json.Serialization;

namespace TradeFlow.Consumer.Models;

public class MarketTick
{
    [JsonPropertyName("T")]
    public long Timestamp { get; set; }

    [JsonPropertyName("s")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("p")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Price { get; set; }

    [JsonPropertyName("q")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Quantity { get; set; }
}