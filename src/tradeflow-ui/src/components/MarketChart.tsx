import {
  createChart,
  ColorType,
  LineSeries,
  type IChartApi,
  type ISeriesApi,
  type Time,
} from "lightweight-charts";
import React, { useEffect, useRef } from "react";
import type { MarketTick } from "../context/SignalRContext";

interface MarketChartProps {
  data: MarketTick | null;
}

export const MarketChart: React.FC<MarketChartProps> = ({ data }) => {
  const chartContainerRef = useRef<HTMLDivElement>(null);
  const chartRef = useRef<IChartApi | null>(null);
  const seriesRef = useRef<ISeriesApi<"Line"> | null>(null);

  useEffect(() => {
    if (!chartContainerRef.current) return;

    const chart = createChart(chartContainerRef.current, {
      layout: {
        background: { type: ColorType.Solid, color: "#1a1a1a" },
        textColor: "#d1d4dc",
      },
      width: chartContainerRef.current.clientWidth,
      height: 400,
      grid: {
        vertLines: { color: "#333" },
        horzLines: { color: "#333" },
      },
      timeScale: {
        timeVisible: true,
        secondsVisible: true,
      },
    });

    const lineSeries = chart.addSeries(LineSeries, {
      color: "#2962FF",
      lineWidth: 2,
    });

    chartRef.current = chart;
    seriesRef.current = lineSeries;

    const handleResize = () => {
      if (chartContainerRef.current) {
        chart.applyOptions({ width: chartContainerRef.current.clientWidth });
      }
    };

    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("resize", handleResize);
      chart.remove();
    };
  }, []);

  useEffect(() => {
    if (seriesRef.current && data) {
      seriesRef.current.update({
        time: (Date.now() / 1000) as Time,
        value: data.price,
      });
    }
  }, [data]);

  return (
    <div ref={chartContainerRef} style={{ width: "100%", height: "400px" }} />
  );
};
