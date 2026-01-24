Write-Host "Starting TradeFlow System..." -ForegroundColor Green

# 1. Start Infrastructure (Docker)
Write-Host "1. Starting Infrastructure (Docker)..."
docker-compose up -d

# 2. Start Ingestor (New Window)
Write-Host "2. Starting Ingestor..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'src\TradeFlow.Ingestor'; dotnet run"

# 3. Start Consumer (New Window)
Write-Host "3. Starting Consumer..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'src\TradeFlow.Consumer'; dotnet run"

# 4. Start Analytics Engine (New Window)
Write-Host "4. Starting Analytics Engine..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'src\analytics-engine'; python main.py"

# 5. Start Frontend (New Window)
Write-Host "5. Starting Frontend..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'src\tradeflow-ui'; npm run dev"

Write-Host "All services started! Access the dashboard at http://localhost:5173" -ForegroundColor Cyan
