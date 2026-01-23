import time
from database import TimescaleRepository
from interfaces import IDataRepository
from datetime import datetime
from sentiment import SentimentAnalyser
from prediction import NumericPredictor

class AnalyticsEngine:
    def __init__(self, repository: IDataRepository, sentiment: ISentimentAnalyser, predictor: IPricePredictor):
        self.repository = repository
        self.sentiment = sentiment
        self.predictor = predictor
        self.headlines = [
            "Bitcoin hits all time high as adoption soars",
            "Regulatory crackdown causes panic in crypto markets",
            "Market sideways as investors wait for economic data",
            "New crypto exchange launches, sparking interest",
            "Market volatility as geopolitical tensions escalate"
        ]
        self.tick = 0

    def run(self):
        print("Analytics Engine Started...")
        
        while True:
            candles = self.repository.fetch_latest_candles(limit=5)

            news = self.headlines[self.tick % len(self.headlines)]
            self.tick += 1

            sentiment_result = self.sentiment.analyse(news)
            label = sentiment_result['label']
            score = round(sentiment_result['score'], 2)

            numeric_score = score if label == 'positive' else -score

            if candles:
                latest = candles[0]
                price = latest['close']

                prediction = self.predictor.predict(candles, numeric_score)

                print(f"[{datetime.now().strftime('%H:%M:%S')}] Close: ${price} | News: '{news}'")
                print(f"AI Analysis: {label.upper()} ({score*100}%) -> Forecast: {prediction}")
                print("-" * 80)
            else:
                print(f"[{datetime.now().strftime('%H:%M:%S')}] Waiting for data...")

            time.sleep(10)

if __name__ == "__main__":
    repo = TimescaleRepository()
    brain = SentimentAnalyser()
    oracle = NumericPredictor()
    engine = AnalyticsEngine(repo, brain, oracle)
    engine.run()
