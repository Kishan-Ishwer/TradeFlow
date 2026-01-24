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
using TradeFlow.Common.Models;

namespace TradeFlow.Consumer.Services;

public class AiRabbitMQListener(IConfiguration config, ILogger<AiRabbitMQListener> logger) : IAiMessageListener, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;

    public Task StartListeningAsync(Func<AiPrediction, Task> onMessageReceived, CancellationToken cancellationToken)
    {
        var hostname = config["RabbitMQ:HostName"] ?? "localhost";
        var port = config.GetValue<int>("RabbitMQ:Port");

        var factory = new ConnectionFactory { HostName = hostname, Port = port, DispatchConsumersAsync = true };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "ai_signals", type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare(queue: "ai_signals_ingest", durable: true, exclusive: false, autoDelete: false, arguments: null).QueueName;
        _channel.QueueBind(queue: queueName, exchange: "ai_signals", routingKey: "");

        logger.LogInformation("Listener connected to RabbitMQ (AI Signals)");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var prediction = JsonSerializer.Deserialize<AiPrediction>(message, options);

                if (prediction != null) 
                {
                    await onMessageReceived(prediction);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing AI Signal");
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
