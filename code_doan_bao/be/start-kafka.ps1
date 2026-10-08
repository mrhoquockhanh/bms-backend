Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Kafka and Zookeeper..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

docker-compose up -d zookeeper kafka

Write-Host ""
Write-Host "Waiting for Kafka to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Checking Kafka status..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
docker ps --filter "name=kafka" --filter "name=zookeeper" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Kafka should be ready now!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "To check logs: docker logs kafka" -ForegroundColor Yellow
Write-Host "To stop: docker-compose down kafka zookeeper" -ForegroundColor Yellow
Write-Host ""



















