using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TradeFlow.Ingestor.Interfaces;

namespace TradeFlow.Ingestor.Services;

public class BinanceClient(ILogger<BinanceClient> logger) : IBinanceClient
{
    private readonly Uri _socketUrl = new("wss://stream.binance.com:9443/ws/btcusdt@trade");

    public async Task ConnectAsync(Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        using var socket = new ClientWebSocket();

        try
        {
            logger.LogInformation("Connection to Binace...");
            await socket.ConnectAsync(_socketUrl, cancellationToken);
            logger.LogInformation("Connected!");

            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await onMessageReceived(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Binance WebSocket Error");
        }
    }
}
