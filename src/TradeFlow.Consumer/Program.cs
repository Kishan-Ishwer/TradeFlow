using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Consumer;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Consumer.Services;
using TradeFlow.Consumer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddSingleton<ITimescaleRepository, TimescaleRepository>();
builder.Services.AddSingleton<IMessageListener, RabbitMQListener>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseCors();
app.MapHub<MarketHub>("/marketHub");

app.Run();