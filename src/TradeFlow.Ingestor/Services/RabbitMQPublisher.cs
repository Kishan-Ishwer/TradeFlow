using System;
using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using TradeFlow.Ingestor.Interfaces;

namespace TradeFlow.Ingestor.Services;

public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    

    public RabbitMQPublisher(IConfiguration config)
    {
        var hostname = config["RabbitMQ:HostName"] ?? "localhost";
        var port = config.GetValue<int>("RabbitMQ:Port");

        var factory = new ConnectionFactory { HostName = hostname, Port = port };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "market_data", type: ExchangeType.Fanout);
    }

    public void Publish(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "market_data",
                            routingKey: "",
                            basicProperties: null,
                            body: body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        GC.SuppressFinalize(this);
    }
}
