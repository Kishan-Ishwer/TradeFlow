# TradeFlow

**TradeFlow** is a high-frequency crypto analytics engine. It ingests real-time market data, processes it with AI, and visualizes it for traders.

## System Components

| Service       | Technology       | Responsibility                                             | Status      |
| :------------ | :--------------- | :--------------------------------------------------------- | :---------- |
| **Ingestor**  | .NET 8 Worker    | Connects to Binance WebSocket and publishes raw trades.    | ✅ Complete |
| **RabbitMQ**  | Broker           | Handles high-throughput message streaming.                 | ✅ Complete |
| **Consumer**  | .NET 8 Worker    | Consumes messages and inserts them into storage.           | ✅ Complete |
| **Storage**   | TimescaleDB      | Stores raw ticks and auto-generates 1-minute candles.      | ✅ Complete |
| **Analytics** | Python (FinBERT) | AI sentiment analysis and price prediction.                | ✅ Complete |
| **Frontend**  | React            | Real-time visualization of ticker, chart, and AI insights. | ✅ Complete |

## Getting Started

## ⚡ Getting Started (Quickly)

We have created automation scripts to manage the entire stack.

### 1-Click Setup (First Run)

Installs Python dependencies, restores .NET projects, installs web packages, and starts everything.

```powershell
.\setup.ps1
```

### 1-Click Launch

Starts Docker, Infrastructure, Backend, AI Engine, and Frontend in separate windows.

```powershell
.\start-all.ps1
```

### Shutdown

Stops Docker containers and kills all associated processes.

```powershell
.\stop-all.ps1
```

---

## Manual Startup

If you prefer running services manually:

### 1. Infrastructure

```bash
docker-compose up -d
```

### 2. Services

Open 4 terminals and run:

**Terminal A (Ingestor)**

```bash
cd src/TradeFlow.Ingestor
dotnet run
```

**Terminal B (Consumer)**

```bash
cd src/TradeFlow.Consumer
dotnet run
```

**Terminal C (Analytics Engine)**

```bash
cd src/analytics-engine
python main.py
```

**Terminal D (Frontend)**

```bash
cd src/tradeflow-ui
npm run dev
```

## Project Structure

```
TradeFlow/
├── docker-compose.yml       # RabbitMQ & TimescaleDB
├── setup.ps1                # 1-Click Setup Script
├── start-all.ps1            # 1-Click Startup Script
├── stop-all.ps1             # 1-Click Shutdown Script
├── database/                # SQL Init Scripts
│   ├── init.sql             # Hypertable setup
│   └── candles.sql          # Continuous Aggregates
├── src/
│   ├── TradeFlow.Ingestor/  # WebSocket -> RabbitMQ
│   ├── TradeFlow.Consumer/  # RabbitMQ -> TimescaleDB
│   ├── analytics-engine/    # Python (FinBERT + Prediction)
│   └── tradeflow-ui/        # React Frontend (Vite)
```
