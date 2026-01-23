# TradeFlow

**TradeFlow** is a high-frequency crypto analytics engine. It ingests real-time market data, processes it with AI, and visualizes it for traders.

## System Components

| Service       | Technology       | Responsibility                                          | Status      |
| :------------ | :--------------- | :------------------------------------------------------ | :---------- |
| **Ingestor**  | .NET 8 Worker    | Connects to Binance WebSocket and publishes raw trades. | ✅ Complete |
| **RabbitMQ**  | Broker           | Handles high-throughput message streaming.              | ✅ Complete |
| **Consumer**  | .NET 8 Worker    | Consumes messages and inserts them into storage.        | ✅ Complete |
| **Storage**   | TimescaleDB      | Stores raw ticks and auto-generates 1-minute candles.   | ✅ Complete |
| **Analytics** | Python (FinBERT) | AI sentiment analysis and price prediction.             | ✅ Complete |
| **Frontend**  | React            | (Planned) Real-time visualization.                      | 🚧 Planned  |

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 1. Launch Infrastructure

Start the message broker and database containers.

```bash
docker-compose up -d
```

### 2. Initialize Database

Execute the SQL scripts to set up the Hypertables and Continuous Aggregates.

```bash
docker cp database/init.sql tradeflow-db:/tmp/init.sql
docker exec tradeflow-db psql -U postgres -d tradeflow -f /tmp/init.sql

docker cp database/candles.sql tradeflow-db:/tmp/candles.sql
docker exec tradeflow-db psql -U postgres -d tradeflow -f /tmp/candles.sql
```

### 3. Start Services

Run the Consumer and Ingestor in separate terminals.

**Terminal A (Consumer)**

```bash
cd src/TradeFlow.Consumer
dotnet run
```

**Terminal B (Ingestor)**

```bash
cd src/TradeFlow.Ingestor
dotnet run
```

**Terminal C (Analytics Engine)**

```bash
cd src/analytics-engine
# Activate venv
.\venv\Scripts\Activate
# Run Engine
python main.py
```

## Project Structure

```
TradeFlow/
├── docker-compose.yml       # RabbitMQ & TimescaleDB
├── database/                # SQL Init Scripts
│   ├── init.sql             # Hypertable setup
│   └── candles.sql          # Continuous Aggregates
├── src/
│   ├── TradeFlow.Ingestor/  # WebSocket -> RabbitMQ
│   ├── TradeFlow.Consumer/  # RabbitMQ -> TimescaleDB
│   └── analytics-engine/    # Python (FinBERT + Prediction)
```
