# TradeFlow 🌊

**TradeFlow** is a high-frequency crypto analytics engine. It ingests real-time market data, processes it with AI, and visualizes it for traders.

## Phase 1: The Backbone 🦴

Currently, the project consists of the **Ingestion Layer** and the **Infrastructure**.

### Architecture

- **TradeFlow.Ingestor**: A .NET 8 Worker Service.
  - Connects to Binance WebSocket (`wss://stream.binance.com:9443/ws/btcusdt@trade`).
  - Publishes raw trade messages to RabbitMQ.
- **RabbitMQ**: Message broker for decoupling services.
- **TimescaleDB**: Time-series database

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Quick Start

1.  **Start Infrastructure**

    ```powershell
    docker-compose up -d
    ```

    - RabbitMQ Dashboard: [http://localhost:15673](http://localhost:15673) (User: `guest`, Pass: `guest`)

2.  **Run Ingestor**
    ```powershell
    cd src/TradeFlow.Ingestor
    dotnet run
    ```
    You should see logs indicating connections to both Binance and RabbitMQ.

### Project Structure

```
TradeFlow/
├── docker-compose.yml       # RabbitMQ & TimescaleDB
├── src/
│   ├── TradeFlow.Ingestor/  # C# Worker Service
│   │   ├── Interfaces/      # IBinanceClient, IMessagePublisher
│   │   ├── Services/        # Concrete Implementations
│   │   ├── Worker.cs        # Coordinator
│   │   └── Program.cs       # DI Wiring
```
