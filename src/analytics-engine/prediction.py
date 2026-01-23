from interfaces import IPricePredictor
from typing import List, Dict, Any

class NumericPredictor(IPricePredictor):
    def predict(self, candles: List[Dict[str, Any]], sentiment_score: float) -> str:
        if not candles:
            return "WAITING"

        latest = candles[0]
        open_price = float(latest['open'])
        close_price = float(latest['close'])
        is_green = close_price > open_price

        if sentiment_score > 0.6:
            return "STRONG BUY" if is_green else "BUY THE DIP"

        elif sentiment_score < -0.6:
            return "SELL THE RALLY" if is_green else "STRONG SELL"

        elif sentiment_score > 0.2:
            return "BUY"

        elif sentiment_score < -0.2:
            return "SELL"

        return "HOLD"