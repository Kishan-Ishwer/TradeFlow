from transformers import pipeline
from interfaces import ISentimentAnalyser
from typing import Dict, Any

class SentimentAnalyser(ISentimentAnalyser):
    def __init__(self):
        print(" Loading FinBERT model... (this might take a while)")
        self.classifier = pipeline("sentiment-analysis", model="ProsusAI/finbert")

    def analyse(self, text: str) -> Dict[str, Any]:
        result = self.classifier(text)[0]
        return result