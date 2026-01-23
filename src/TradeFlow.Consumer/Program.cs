using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Consumer;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Consumer.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ITimescaleRepository, TimescaleRepository>();
builder.Services.AddSingleton<IMessageListener, RabbitMQListener>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
