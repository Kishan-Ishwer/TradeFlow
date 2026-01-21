namespace TradeFlow.Ingestor.Interfaces;

public interface IBinanceClient
{
    Task ConnectAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken);
}
