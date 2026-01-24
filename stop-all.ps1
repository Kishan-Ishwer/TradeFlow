Write-Host "Stopping TradeFlow System..." -ForegroundColor Yellow

# 1. Stop Infrastructure
Write-Host "1. Stopping Docker Containers..."
docker-compose down

# 2. Kill .NET Processes (Backend)
Write-Host "2. Stopping Backend Services..."
Get-Process "TradeFlow.Ingestor" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process "TradeFlow.Consumer" -ErrorAction SilentlyContinue | Stop-Process -Force

# 3. Kill Process on Port 5000 (Consumer) if still alive
$port5000 = Get-NetTCPConnection -LocalPort 5000 -ErrorAction SilentlyContinue
if ($port5000) {
    Write-Host "   - Freeing Port 5000..."
    Stop-Process -Id $port5000.OwningProcess -Force -ErrorAction SilentlyContinue
}

# 4. Kill Process on Port 5173 (Frontend)
$port5173 = Get-NetTCPConnection -LocalPort 5173 -ErrorAction SilentlyContinue
if ($port5173) {
    Write-Host "3. Stopping Frontend (Port 5173)..."
    Stop-Process -Id $port5173.OwningProcess -Force -ErrorAction SilentlyContinue
}

# 5. Python (Hard to identify specific python process safely)
Write-Host "4. NOTE: Please manually close the Python Analytics terminal window." -ForegroundColor Cyan

Write-Host "TradeFlow System Shutdown Complete." -ForegroundColor Green
