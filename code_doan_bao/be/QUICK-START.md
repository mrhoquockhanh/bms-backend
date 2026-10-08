# Quick Start Guide - Kafka & gRPC Integration

## 🚀 Bước 1: Start Dependencies

```bash
# Windows
.\start-dependencies.bat

# Hoặc manual
docker-compose up -d zookeeper kafka consul postgres
```

**Kiểm tra:**
```bash
.\check-dependencies.bat
```

## 🚀 Bước 2: Start Services

### iam-service
```bash
cd iam-service
dotnet run
```

**Expected logs:**
```
✅ [Kafka] Topic exists: iam-service.user-created
✅ [IAMService] Kafka producer configured on localhost:29092
✅ [Consul] Service registered successfully
Now listening on: http://[::]:7020 (REST)
Now listening on: http://[::]:7021 (gRPC)
```

### Patient-service
```bash
cd Patient-service
dotnet run
```

**Expected logs:**
```
🟢 Kafka consumer started. Listening on topic: iam-service.user-created
Now listening on: http://[::]:7263
```

## 🧪 Bước 3: Test Integration

### Test Kafka (Event-driven)

1. **Create user via iam-service:**
```bash
POST http://localhost:7020/api/auth/register
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}
```

2. **Check Patient-service logs:**
```
✅ [Kafka] Received UserCreatedEvent for test@example.com
📦 [PatientService] Syncing user test@example.com into Patient DB
```

### Test gRPC (Synchronous)

**Option 1: Using IamGrpcClient**
```csharp
// In Patient-service controller
[HttpGet("test-grpc/{userId}")]
public async Task<IActionResult> TestGrpc(string userId)
{
    var grpcClient = HttpContext.RequestServices.GetRequiredService<IamGrpcClient>();
    var user = await grpcClient.GetUserByIdAsync(userId);
    return Ok(user);
}
```

**Option 2: Direct API call**
```bash
GET http://localhost:7263/api/test-grpc/ACC-1234
```

## 🔍 Troubleshooting

### Issue: Kafka connection failed
**Solution:**
```bash
# Check if Kafka is running
docker ps | findstr kafka

# If not, start it
docker-compose up -d kafka zookeeper

# Wait 10-15 seconds
docker logs kafka --tail 20
```

### Issue: Database connection failed
**Solution:**
- **iam-service**: Ensure PostgreSQL is running on `localhost:5432`
- **Patient-service**: Ensure SQL Server is running on `localhost`

### Issue: Consul connection failed
**Solution:**
```bash
# Start Consul
docker-compose up -d consul

# Verify
curl http://localhost:8500/v1/status/leader
```

## 📊 Service Endpoints

### iam-service
- REST API: http://localhost:7020
- gRPC: http://localhost:7021
- Swagger: http://localhost:7020
- Health: http://localhost:7020/health

### Patient-service
- REST API: http://localhost:7263
- Swagger: http://localhost:7263
- Health: http://localhost:7263/

## 🔗 Integration Points

### Kafka Topics
- **Topic**: `iam-service.user-created`
- **Producer**: iam-service (when user is created)
- **Consumer**: Patient-service (listens for new users)

### gRPC Services
- **Server**: iam-service (port 7021)
- **Client**: Patient-service (via IamGrpcClient)
- **Methods**: 
  - `GetUserById`
  - `GetUserByEmail`
  - `ValidateUserToken`

## ✅ Verification Checklist

- [ ] Docker is running
- [ ] Zookeeper container is running
- [ ] Kafka container is running and healthy
- [ ] Consul container is running (optional)
- [ ] PostgreSQL is running (for iam-service)
- [ ] SQL Server is running (for Patient-service)
- [ ] iam-service started successfully
- [ ] Patient-service started successfully
- [ ] Kafka topic created/verified
- [ ] Consul service registered (if using)

## 🎯 Expected Behavior

### When ALL dependencies are running:
- ✅ No connection errors
- ✅ Kafka topic exists/created
- ✅ Consul service registered
- ✅ Services ready to accept requests

### When dependencies are NOT running:
- ⚠️ Connection errors (expected)
- ✅ Services still start successfully
- ✅ Auto-retry when dependencies become available
- ✅ Graceful fallback




















