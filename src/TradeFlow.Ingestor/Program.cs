using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Ingestor;
using TradeFlow.Ingestor.Interfaces;
using TradeFlow.Ingestor.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
builder.Services.AddSingleton<IBinanceClient, BinanceClient>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
