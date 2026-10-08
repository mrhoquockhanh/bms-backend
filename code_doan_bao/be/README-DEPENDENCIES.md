# Dependencies Setup Guide

## Quick Start

### Windows
```bash
# Option 1: Using batch file
start-dependencies.bat

# Option 2: Using PowerShell
.\start-dependencies.ps1

# Option 3: Manual
docker-compose up -d zookeeper kafka consul
```

### Linux/Mac
```bash
docker-compose up -d zookeeper kafka consul
```

## Services

### 1. Zookeeper
- **Port**: 2181
- **Purpose**: Required by Kafka for coordination
- **Start**: `docker-compose up -d zookeeper`

### 2. Kafka
- **Ports**: 
  - 9092 (internal/Docker network)
  - 29092 (external/localhost)
- **Purpose**: Message broker for event-driven communication
- **Start**: `docker-compose up -d kafka`
- **UI**: http://localhost:8085 (Kafka UI)

### 3. Consul
- **Port**: 8500
- **Purpose**: Service discovery and configuration
- **Start**: `docker-compose up -d consul`
- **UI**: http://localhost:8500

## Verify Services

### Check if services are running:
```bash
docker ps | grep -E "kafka|zookeeper|consul"
```

### Check Kafka connectivity:
```bash
# Test connection
telnet localhost 29092
# or
nc -zv localhost 29092
```

### Check Consul:
```bash
# Open browser
http://localhost:8500
```

## Troubleshooting

### Kafka connection issues

**Problem**: `Connect to localhost:9092 failed`

**Solution**:
1. Ensure Kafka is running: `docker ps | grep kafka`
2. If not running: `docker-compose up -d kafka zookeeper`
3. Wait 10-15 seconds for Kafka to fully start
4. Check logs: `docker logs kafka`

**Note**: When running services locally (not in Docker), use port `29092` instead of `9092`:
```json
{
  "Kafka": {
    "Host": "localhost:29092"
  }
}
```

### Consul connection issues

**Problem**: `No such host is known. (consul:8500)`

**Solution**:
1. If running locally (not in Docker), update `appsettings.json`:
```json
{
  "ConsulConfig": {
    "Address": "http://localhost:8500"
  }
}
```

2. Start Consul: `docker-compose up -d consul`

## Service Dependencies

```
iam-service
  ├── Kafka (for event publishing)
  ├── Consul (for service registration)
  └── PostgreSQL (database)

patient-service
  ├── Kafka (for event consumption)
  ├── Consul (for service discovery)
  └── SQL Server (database)
```

## Development vs Production

### Development (Local)
- Kafka: `localhost:29092`
- Consul: `localhost:8500`
- Services run directly (not in Docker)

### Production (Docker)
- Kafka: `kafka:9092` (Docker network)
- Consul: `consul:8500` (Docker network)
- All services in Docker Compose

## Auto-start on System Boot

### Windows (Task Scheduler)
1. Open Task Scheduler
2. Create Basic Task
3. Action: Start a program
4. Program: `docker-compose`
5. Arguments: `up -d zookeeper kafka consul`
6. Trigger: At startup

### Linux (systemd)
Create `/etc/systemd/system/docker-dependencies.service`:
```ini
[Unit]
Description=Docker Dependencies (Kafka, Consul)
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/path/to/project
ExecStart=/usr/bin/docker-compose up -d zookeeper kafka consul
ExecStop=/usr/bin/docker-compose stop zookeeper kafka consul

[Install]
WantedBy=multi-user.target
```

Then:
```bash
sudo systemctl enable docker-dependencies
sudo systemctl start docker-dependencies
```

