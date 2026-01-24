Write-Host "Initializing TradeFlow Setup..." -ForegroundColor Green

# 1. Check Docker
Write-Host "1. Checking Docker..."
if (!(docker info)) {
    Write-Error "Docker is not running. Please start Docker Desktop and run this script again."
    exit
}

# 1. Start Infrastructure (Docker)
Write-Host "1. Starting Infrastructure..."
docker-compose up -d

Write-Host "   Waiting for Database to initialize (10s)..."
Start-Sleep -Seconds 10
# TODO: A better check would be a loop trying 'pg_isready' but fixed wait is okay for a simple setup script

# Apply Schema
Write-Host "   Applying Database Schema..."
Get-Content database\init.sql | docker exec -i tradeflow-db psql -U postgres -d tradeflow
Get-Content database\candles.sql | docker exec -i tradeflow-db psql -U postgres -d tradeflow

# 3. Python Setup
Write-Host "2. Installing Python Dependencies..."
cd src\analytics-engine
pip install -r requirements.txt
cd ..\..

# 4. .NET Restores
Write-Host "3. Restoring .NET Projects..."
dotnet restore src\TradeFlow.Ingestor\TradeFlow.Ingestor.csproj
dotnet restore src\TradeFlow.Consumer\TradeFlow.Consumer.csproj

# 5. Frontend Setup
Write-Host "4. Installing Frontend Dependencies..."
cd src\tradeflow-ui
npm install
cd ..\..

Write-Host "Setup Complete! Launching TradeFlow..." -ForegroundColor Cyan

# 6. Launch Everything
.\start-all.ps1
