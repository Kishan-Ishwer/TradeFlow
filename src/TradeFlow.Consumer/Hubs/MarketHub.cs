using Microsoft.AspNetCore.SignalR;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Consumer.Models;

namespace TradeFlow.Consumer.Hubs;

public class MarketHub(ITimescaleRepository repository) : Hub
{
    public async Task<List<MarketTick>> GetMarketHistory()
    {
        return await repository.GetRecentTicksAsync(100);
    }
}