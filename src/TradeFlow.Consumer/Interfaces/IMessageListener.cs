using TradeFlow.Consumer.Models;

namespace TradeFlow.Consumer.Interfaces;

public interface IMessageListener
{
    Task StartListeningAsync(Func<MarketTick, Task> onMessageReceived, CancellationToken cancellationToken);
}