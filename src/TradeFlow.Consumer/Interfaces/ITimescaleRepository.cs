using TradeFlow.Consumer.Models;

namespace TradeFlow.Consumer.Interfaces;

public interface ITimescaleRepository
{
    Task InsertTickAsync(MarketTick tick);
}