import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { SignalRProvider } from "./context/SignalRContext.tsx";
import { ErrorBoundary } from "./components/ErrorBoundary.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ErrorBoundary>
      <SignalRProvider>
        <App />
      </SignalRProvider>
    </ErrorBoundary>
  </StrictMode>,
);
