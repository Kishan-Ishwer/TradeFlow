CREATE TABLE IF NOT EXISTS MARKET_TICKS (
    time TIMESTAMPTZ NOT NULL,
    symbol TEXT NOT NULL,
    price DOUBLE PRECISION NULL,
    quantity DOUBLE PRECISION NULL
);

SELECT create_hypertable('market_ticks', 'time', if_not_exists => TRUE);