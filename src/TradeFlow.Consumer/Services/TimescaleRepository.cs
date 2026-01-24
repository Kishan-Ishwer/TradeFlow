using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using TradeFlow.Consumer.Interfaces;
using TradeFlow.Common.Models;

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

    public async Task<List<MarketTick>> GetRecentTicksAsync(int limit)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var sql = @"
            SELECT time, symbol, price, quantity 
            FROM market_ticks 
            ORDER BY time DESC 
            LIMIT @limit";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("limit", limit);

        using var reader = await cmd.ExecuteReaderAsync();
        var ticks = new List<MarketTick>();

        while (await reader.ReadAsync())
        {
            ticks.Add(new MarketTick
            {
                Timestamp = new DateTimeOffset(reader.GetDateTime(0)).ToUnixTimeMilliseconds(),
                Symbol = reader.GetString(1),
                Price = (decimal)reader.GetDouble(2),
                Quantity = (decimal)reader.GetDouble(3)
            });
        }

        ticks.Reverse();
        return ticks;
    }
}