using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Consumer.Services;

namespace TradeFlow.Consumer;

public class Worker(IMessageListener listener, ITimescaleRepository repository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await listener.StartListeningAsync(async (tick) => 
        {
            await repository.InsertTickAsync(tick);
        }, stoppingToken);

         await Task.Delay(Timeout.Infinite, stoppingToken);
    }   
}

   
