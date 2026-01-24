import { useSignalR } from "./context/SignalRContext";
import { MarketChart } from "./components/MarketChart";
import "./App.css";

function App() {
  console.log("App Component Rendering...");
  const { isConnected, lastTick, aiPrediction } = useSignalR();

  return (
    <div className="container">
      <h1>TradeFlow Dashboard 🚀</h1>
      <div className={`status ${isConnected ? "connected" : "disconnected"}`}>
        {isConnected ? "🟢 Connected to SignalR" : "🔴 Connecting..."}
      </div>

      <div className="dashboard-content">
        <div
          style={{
            display: "flex",
            gap: "20px",
            width: "100%",
            justifyContent: "center",
          }}
        >
          {/* Ticker Card */}
          {lastTick && (
            <div className="ticker-card">
              <h2>{lastTick.symbol}</h2>
              <p className="price">
                $
                {typeof lastTick.price === "number"
                  ? lastTick.price.toFixed(2)
                  : lastTick.price}
              </p>
              <p className="timestamp">{lastTick.timestamp}</p>
            </div>
          )}

          {/* AI Prediction Card */}
          {aiPrediction && (
            <div
              className="ticker-card"
              style={{ border: "1px solid #7c4dff" }}
            >
              <h2 style={{ color: "#b388ff" }}>AI Insight 🧠</h2>
              <p style={{ fontSize: "1.2rem", marginTop: "10px" }}>
                {aiPrediction.news}
              </p>
              <div
                style={{
                  margin: "15px 0",
                  fontSize: "1.5rem",
                  fontWeight: "bold",
                  color:
                    aiPrediction.label === "positive" ? "#69f0ae" : "#ff5252",
                }}
              >
                {aiPrediction.label.toUpperCase()} (
                {(aiPrediction.score * 100).toFixed(0)}%)
              </div>
              <p style={{ color: "#eee" }}>
                Forecast: {aiPrediction.prediction}
              </p>
            </div>
          )}
        </div>

        <div className="chart-container">
          <MarketChart data={lastTick} />
        </div>
      </div>
    </div>
  );
}

export default App;
