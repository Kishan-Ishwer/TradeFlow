using Npgsql;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Consumer.Models;

namespace TradeFlow.Consumer.Services;

public class TimescaleRepository(IConfiguration config) : ITimescaleRepository
{
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection")!;

    public async Task InsertTickAsync(MarketTick tick)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var sql = @"
            INSERT INTO market_ticks (time, symbol, price, quantity)
            VALUES (@t, @s, @p, @q)";

        using var cmd = new NpgsqlCommand(sql, conn);

        var time = DateTimeOffset.FromUnixTimeMilliseconds(tick.Timestamp).UtcDateTime;

        cmd.Parameters.AddWithValue("t", time);
        cmd.Parameters.AddWithValue("s", tick.Symbol);
        cmd.Parameters.AddWithValue("p", tick.Price);
        cmd.Parameters.AddWithValue("q", tick.Quantity);

        await cmd.ExecuteNonQueryAsync();
    }
}