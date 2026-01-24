using System;
using System.Threading;
using System.Threading.Tasks;
using TradeFlow.Consumer.Models;

namespace TradeFlow.Consumer.Interfaces;

public interface IAiMessageListener
{
    Task StartListeningAsync(Func<AiPrediction, Task> onMessageReceived, CancellationToken cancellationToken);
}
