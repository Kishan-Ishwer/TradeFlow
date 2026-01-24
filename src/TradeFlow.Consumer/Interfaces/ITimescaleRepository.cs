using System.Threading.Tasks;
using TradeFlow.Common.Models;

namespace TradeFlow.Consumer.Interfaces;

public interface ITimescaleRepository
{
    Task InsertTickAsync(MarketTick tick);
    Task<List<MarketTick>> GetRecentTicksAsync(int limit);
}