using TradeFlow.Ingestor.Interfaces;

namespace TradeFlow.Ingestor;

public class Worker(ILogger<Worker> logger, IBinanceClient binanceClient, IMessagePublisher publisher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Ingestor Service Starting...");

        await binanceClient.ConnectAsync(async (message) =>
        {
            publisher.Publish(message);

            await Task.CompletedTask;
        }, stoppingToken);
    }
}