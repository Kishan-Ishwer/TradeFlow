using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Consumer.Models;

namespace TradeFlow.Consumer.Services;

public class RabbitMQListener(IConfiguration config, ILogger<RabbitMQListener> logger) : IMessageListener, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;

    public Task StartListeningAsync(Func<MarketTick, Task> onMessageReceived, CancellationToken cancellationToken)
    {

        var hostname = config["RabbitMQ:HostName"] ?? "localhost";
        var port = config.GetValue<int>("RabbitMQ:Port");

        var factory = new ConnectionFactory { HostName = hostname, Port = port, DispatchConsumersAsync = true };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "market_data", type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare(queue: "timescale_ingest", durable: true, exclusive: false, autoDelete: false, arguments: null).QueueName;
        _channel.QueueBind(queue: queueName, exchange: "market_data", routingKey: "");

        logger.LogInformation("Listener connected to RabbitMQ");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var tick = JsonSerializer.Deserialize<MarketTick>(message, options);

                if (tick != null) 
                {
                    await onMessageReceived(tick);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing message");
            }
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        GC.SuppressFinalize(this);
    }
}