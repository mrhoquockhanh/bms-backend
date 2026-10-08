Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Dependencies (Kafka, Consul)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/3] Starting Zookeeper..." -ForegroundColor Yellow
docker-compose up -d zookeeper
Start-Sleep -Seconds 3

Write-Host "[2/3] Starting Kafka..." -ForegroundColor Yellow
docker-compose up -d kafka
Start-Sleep -Seconds 5

Write-Host "[3/3] Starting Consul..." -ForegroundColor Yellow
docker-compose up -d consul
Start-Sleep -Seconds 3

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Checking services status..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
docker ps --filter "name=kafka" --filter "name=zookeeper" --filter "name=consul" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Waiting for services to be ready..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "✅ Dependencies started!" -ForegroundColor Green
Write-Host ""
Write-Host "Kafka UI: http://localhost:8085" -ForegroundColor Cyan
Write-Host "Consul UI: http://localhost:8500" -ForegroundColor Cyan
Write-Host ""




















