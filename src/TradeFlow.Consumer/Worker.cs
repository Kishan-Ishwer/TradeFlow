using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using TradeFlow.Consumer.Hubs;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Consumer.Services;

namespace TradeFlow.Consumer;

public class Worker(IMessageListener listener, IAiMessageListener aiListener, ITimescaleRepository repository, IHubContext<MarketHub> hubContext) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await listener.StartListeningAsync(async (tick) => 
        {
            await repository.InsertTickAsync(tick);
            await hubContext.Clients.All.SendAsync("ReceiveTick", tick, stoppingToken);
        }, stoppingToken);

        await aiListener.StartListeningAsync(async (prediction) =>
        {
            await hubContext.Clients.All.SendAsync("ReceivePrediction", prediction, stoppingToken);
        }, stoppingToken);

         await Task.Delay(Timeout.Infinite, stoppingToken);
    }   
}

   
