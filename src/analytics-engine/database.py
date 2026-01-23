from interfaces import IDataRepository
import os
import psycopg2
from psycopg2.extras import RealDictCursor
from typing import List, Dict, Any
from dotenv import load_dotenv

load_dotenv()

class TimescaleRepository(IDataRepository):
    def __init__(self):
        self.host = os.getenv('DB_HOST', 'localhost')
        self.db_name = os.getenv('DB_NAME', 'tradeflow')
        self.user = os.getenv('DB_USER', 'postgres')
        self.password = os.getenv('DB_PASS', 'password')
        self.port = os.getenv('DB_PORT', '5435')

    def _get_connection(self):
        return psycopg2.connect(
            host=self.host,
            database=self.db_name,
            user=self.user,
            password=self.password,
            port=self.port
        )

    def fetch_latest_candles(self, symbol: str = "BTCUSDT", limit: int = 5) -> List[Dict[str, Any]]:
        conn = self._get_connection()
        try:
            with conn.cursor(cursor_factory=RealDictCursor) as cur:
                cur.execute("""
                    SELECT bucket as time, open, high, low, close, volume
                    FROM candles_1m
                    WHERE symbol = %s
                    ORDER BY bucket DESC
                    LIMIT %s;
                """, (symbol, limit))
                return cur.fetchall()
        
        except Exception as e:
            print(f"Error fetching candles: {e}")
            return []

        finally:
            conn.close()