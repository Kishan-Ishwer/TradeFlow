using System;
using System.Threading;
using System.Threading.Tasks;
using TradeFlow.Common.Models;

namespace TradeFlow.Consumer.Interfaces;

public interface IMessageListener
{
    Task StartListeningAsync(Func<MarketTick, Task> onMessageReceived, CancellationToken cancellationToken);
}