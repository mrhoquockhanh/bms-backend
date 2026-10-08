@echo off
echo ========================================
echo Starting Dependencies (Kafka, Consul)
echo ========================================
echo.

echo [1/3] Starting Zookeeper...
docker-compose up -d zookeeper
timeout /t 3 /nobreak >nul

echo [2/3] Starting Kafka...
docker-compose up -d kafka
timeout /t 5 /nobreak >nul

echo [3/3] Starting Consul...
docker-compose up -d consul
timeout /t 3 /nobreak >nul

echo.
echo ========================================
echo Checking services status...
echo ========================================
docker ps --filter "name=kafka" --filter "name=zookeeper" --filter "name=consul" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

echo.
echo ========================================
echo Waiting for services to be ready...
echo ========================================
timeout /t 10 /nobreak >nul

echo.
echo ✅ Dependencies started!
echo.
echo Kafka UI: http://localhost:8085
echo Consul UI: http://localhost:8500
echo.
pause

