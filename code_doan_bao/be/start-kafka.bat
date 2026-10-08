@echo off
echo ========================================
echo Starting Kafka and Zookeeper...
echo ========================================
echo.

docker-compose up -d zookeeper kafka

echo.
echo Waiting for Kafka to be ready...
timeout /t 10 /nobreak >nul

echo.
echo ========================================
echo Checking Kafka status...
echo ========================================
docker ps --filter "name=kafka" --filter "name=zookeeper" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

echo.
echo ========================================
echo Kafka should be ready now!
echo ========================================
echo.
echo To check logs: docker logs kafka
echo To stop: docker-compose down kafka zookeeper
echo.

pause













