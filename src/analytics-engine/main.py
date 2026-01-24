import time
import json
import pika
from database import TimescaleRepository
from interfaces import IDataRepository, ISentimentAnalyser, IPricePredictor, IMessagePublisher
from datetime import datetime
from sentiment import SentimentAnalyser
from prediction import NumericPredictor

class RabbitMQPublisher(IMessagePublisher):
    def __init__(self, host='localhost', port=5672):
        self.connection = pika.BlockingConnection(pika.ConnectionParameters(host=host, port=port))
        self.channel = self.connection.channel()
        self.channel.exchange_declare(exchange='ai_signals', exchange_type='fanout')
        print(f"Connected to RabbitMQ at {host}:{port}")
    def publish(self, message: dict):
        body = json.dumps(message)
        self.channel.basic_publish(exchange='ai_signals', routing_key='', body=body)

class AnalyticsEngine:
    def __init__(self, repository: IDataRepository, sentiment: ISentimentAnalyser, predictor: IPricePredictor, publisher: IMessagePublisher):
        self.repository = repository
        self.sentiment = sentiment
        self.predictor = predictor
        self.publisher = publisher
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
                timestamp = datetime.now().isoformat()
                signal_data = {
                    "Type": "Signal",
                    "Label": label,
                    "Score": score,
                    "Prediction": prediction,
                    "News": news,
                    "Timestamp": timestamp
                }
                # Polymorphic call - engine doesn't care it's RabbitMQ
                self.publisher.publish(signal_data)
                print(f"[{datetime.now().strftime('%H:%M:%S')}] {label.upper()} ({int(score*100)}%) -> {prediction}")
            else:
                print(f"[{datetime.now().strftime('%H:%M:%S')}] Waiting for data...")
            time.sleep(5)


if __name__ == "__main__":
    repo = TimescaleRepository()
    brain = SentimentAnalyser()
    oracle = NumericPredictor()
    pub = RabbitMQPublisher(host='localhost', port=5674)
    engine = AnalyticsEngine(repo, brain, oracle, pub)
    engine.run()
