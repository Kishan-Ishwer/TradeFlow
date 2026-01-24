from abc import ABC, abstractmethod
from typing import List, Dict, Any

class IDataRepository(ABC):
    @abstractmethod
    def fetch_latest_candles(self, symbol: str, limit: int) -> List[Dict[str, Any]]:
        pass


class ISentimentAnalyser(ABC):
    @abstractmethod
    def analyse(self, text: str) -> Dict[str, Any]:
        pass

class IPricePredictor(ABC):
    @abstractmethod
    def predict(self, candles: List[Dict[str, Any]], sentiment_score: float) -> str:
        pass

class IMessagePublisher(ABC):
    @abstractmethod
    def publish(self, message: Dict[str, Any]):
        pass
