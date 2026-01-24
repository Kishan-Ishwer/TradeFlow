import React, { createContext, useContext, useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

export interface MarketTick {
  symbol: string;
  price: number;
  quantity: number;
  timestamp: string;
  rawTimestamp: number;
}

export interface AiPrediction {
  type: string;
  label: string;
  score: number;
  prediction: string;
  news: string;
  timestamp: string;
}

interface ISignalRContext {
  connection: signalR.HubConnection | null;
  lastTick: MarketTick | null;
  history: MarketTick[];
  aiPrediction: AiPrediction | null;
  isConnected: boolean;
}

const SignalRContext = createContext<ISignalRContext | undefined>(undefined);

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );
  const [lastTick, setLastTick] = useState<MarketTick | null>(null);
  const [history, setHistory] = useState<MarketTick[]>([]);
  const [aiPrediction, setAiPrediction] = useState<AiPrediction | null>(null);
  const [isConnected, setIsConnected] = useState(false);

  // Raw interface matching the JSON sent by the backend (C# JsonPropertyName attributes)
  interface RawMarketTick {
    T: number;
    s: string;
    p: number;
    q: number;
  }

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/hubs/market")
      .withAutomaticReconnect()
      .build();

    newConnection.on("ReceiveTick", (rawTick: RawMarketTick) => {
      // Map raw data to friendly MarketTick format
      const tick: MarketTick = {
        symbol: rawTick.s,
        price: rawTick.p,
        quantity: rawTick.q,
        timestamp: new Date(rawTick.T).toLocaleTimeString(),
        rawTimestamp: rawTick.T,
      };
      setLastTick(tick);
    });

    newConnection.on("ReceivePrediction", (prediction: AiPrediction) => {
      console.log("AI Prediction Received:", prediction);
      setAiPrediction(prediction);
    });

    newConnection
      .start()
      .then(() => {
        console.log("SignalR Connected");
        setIsConnected(true);
        setConnection(newConnection);

        // Fetch history
        newConnection
          .invoke("GetMarketHistory")
          .then((ticks: MarketTick[]) => {
            console.log("Fetched history:", ticks.length);
            // Map raw history data
            const mappedHistory = (ticks as unknown as RawMarketTick[]).map(
              (r) => ({
                symbol: r.s,
                price: r.p,
                quantity: r.q,
                timestamp: new Date(r.T).toLocaleTimeString(),
                rawTimestamp: r.T,
              }),
            );
            setHistory(mappedHistory);
          })
          .catch((err) => console.error("Error fetching history:", err));
      })
      .catch((err) => console.error("SignalR Error:", err));

    return () => {
      newConnection.stop();
    };
  }, []);

  return (
    <SignalRContext.Provider
      value={{ connection, lastTick, history, aiPrediction, isConnected }}
    >
      {children}
    </SignalRContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useSignalR = () => {
  const context = useContext(SignalRContext);
  if (!context) {
    throw new Error("useSignalR must be used within a SignalRProvider");
  }
  return context;
};
