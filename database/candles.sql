CREATE MATERIALIZED VIEW candles_1m
WITH (timescaledb.continuous) AS
SELECT
    time_bucket('1 minute', time) AS bucket,
    symbol,
    FIRST(price, time) as open,
    MAX(price) as high,
    MIN(price) as low,
    LAST(price, time) as close,
    SUM(quantity) as volume
FROM market_ticks
GROUP BY bucket, symbol;

SELECT add_continuous_aggregate_policy('candles_1m',
    start_offset => INTERVAL '1 day',
    end_offset => INTERVAL '1 minute',
    schedule_interval => INTERVAL '1 minute');
