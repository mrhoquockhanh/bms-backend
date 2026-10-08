Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Checking Dependencies Status" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check Docker
Write-Host "[1/4] Checking Docker..." -ForegroundColor Yellow
$dockerRunning = docker ps 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker is not running or not installed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Docker is running" -ForegroundColor Green
Write-Host ""

# Check Kafka
Write-Host "[2/4] Checking Kafka..." -ForegroundColor Yellow
$kafka = docker ps --filter "name=kafka" --format "{{.Names}}\t{{.Status}}"
if ($kafka) {
    Write-Host "✅ Kafka is running:" -ForegroundColor Green
    Write-Host "   $kafka" -ForegroundColor Gray
    
    # Test port
    $tcpClient = New-Object System.Net.Sockets.TcpClient
    try {
        $tcpClient.Connect("localhost", 29092)
        Write-Host "✅ Port 29092 is accessible" -ForegroundColor Green
        $tcpClient.Close()
    }
    catch {
        Write-Host "⚠️ Port 29092 is not accessible" -ForegroundColor Yellow
        Write-Host "   Kafka might still be starting up..." -ForegroundColor Gray
    }
}
else {
    Write-Host "❌ Kafka is not running" -ForegroundColor Red
    Write-Host "   Start with: docker-compose up -d kafka zookeeper" -ForegroundColor Yellow
}
Write-Host ""

# Check Zookeeper
Write-Host "[3/4] Checking Zookeeper..." -ForegroundColor Yellow
$zookeeper = docker ps --filter "name=zookeeper" --format "{{.Names}}\t{{.Status}}"
if ($zookeeper) {
    Write-Host "✅ Zookeeper is running:" -ForegroundColor Green
    Write-Host "   $zookeeper" -ForegroundColor Gray
}
else {
    Write-Host "❌ Zookeeper is not running" -ForegroundColor Red
    Write-Host "   Start with: docker-compose up -d zookeeper" -ForegroundColor Yellow
}
Write-Host ""

# Check Consul
Write-Host "[4/4] Checking Consul..." -ForegroundColor Yellow
$consul = docker ps --filter "name=consul" --format "{{.Names}}\t{{.Status}}"
if ($consul) {
    Write-Host "✅ Consul is running:" -ForegroundColor Green
    Write-Host "   $consul" -ForegroundColor Gray
    
    # Test HTTP
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:8500/v1/status/leader" -TimeoutSec 2 -UseBasicParsing
        Write-Host "✅ Consul API is accessible" -ForegroundColor Green
    }
    catch {
        Write-Host "⚠️ Consul API is not accessible" -ForegroundColor Yellow
    }
}
else {
    Write-Host "❌ Consul is not running" -ForegroundColor Red
    Write-Host "   Start with: docker-compose up -d consul" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$allRunning = $kafka -and $zookeeper -and $consul
if ($allRunning) {
    Write-Host "✅ All dependencies are running!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Kafka UI: http://localhost:8085" -ForegroundColor Cyan
    Write-Host "Consul UI: http://localhost:8500" -ForegroundColor Cyan
}
else {
    Write-Host "⚠️ Some dependencies are missing" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To start all dependencies:" -ForegroundColor Yellow
    Write-Host "  docker-compose up -d zookeeper kafka consul" -ForegroundColor White
    Write-Host ""
    Write-Host "Or use the script:" -ForegroundColor Yellow
    Write-Host "  .\start-dependencies.bat" -ForegroundColor White
}




















