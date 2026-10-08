@echo off
echo ========================================
echo Checking Dependencies Status
echo ========================================
echo.

echo [1/4] Checking Docker...
docker ps >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running or not installed
    pause
    exit /b 1
)
echo ✅ Docker is running
echo.

echo [2/4] Checking Kafka...
docker ps --filter "name=kafka" --format "{{.Names}}\t{{.Status}}" >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Kafka is running:
    docker ps --filter "name=kafka" --format "   {{.Names}}\t{{.Status}}"
    
    REM Test port (requires PowerShell or telnet)
    powershell -Command "$tcp = New-Object System.Net.Sockets.TcpClient; try { $tcp.Connect('localhost', 29092); Write-Host '✅ Port 29092 is accessible' -ForegroundColor Green; $tcp.Close() } catch { Write-Host '⚠️ Port 29092 is not accessible' -ForegroundColor Yellow }" 2>nul
) else (
    echo ❌ Kafka is not running
    echo    Start with: docker-compose up -d kafka zookeeper
)
echo.

echo [3/4] Checking Zookeeper...
docker ps --filter "name=zookeeper" --format "{{.Names}}\t{{.Status}}" >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Zookeeper is running:
    docker ps --filter "name=zookeeper" --format "   {{.Names}}\t{{.Status}}"
) else (
    echo ❌ Zookeeper is not running
    echo    Start with: docker-compose up -d zookeeper
)
echo.

echo [4/4] Checking Consul...
docker ps --filter "name=consul" --format "{{.Names}}\t{{.Status}}" >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Consul is running:
    docker ps --filter "name=consul" --format "   {{.Names}}\t{{.Status}}"
) else (
    echo ❌ Consul is not running
    echo    Start with: docker-compose up -d consul
)
echo.

echo ========================================
echo Summary
echo ========================================
echo.

docker ps --filter "name=kafka" --filter "name=zookeeper" --filter "name=consul" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" 2>nul

echo.
echo Kafka UI: http://localhost:8085
echo Consul UI: http://localhost:8500
echo.
pause




















