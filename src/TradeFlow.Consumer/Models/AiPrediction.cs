using System.Text.Json.Serialization;

namespace TradeFlow.Consumer.Models;

public class AiPrediction
{
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Prediction { get; set; } = string.Empty;
    public string News { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
}
