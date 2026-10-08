# Các Vấn Đề Đã Được Sửa - Kafka & gRPC Integration

## 📋 Tóm Tắt Các Vấn Đề Từ Log

### 🔴 **Vấn Đề Nghiêm Trọng:**

1. **Database Connection Error (iam-service)**
   - **Lỗi**: `No such host is known. (postgres:5432)`
   - **Nguyên nhân**: Connection string dùng `postgres` (Docker hostname) khi service chạy local
   - **Đã sửa**: ✅ Auto-detect environment và thay đổi host tự động

2. **Database Connection String Format (Patient-service)**
   - **Lỗi**: Connection string dùng PostgreSQL format nhưng service dùng SQL Server
   - **Đã sửa**: ✅ Đổi sang SQL Server connection string format

3. **Kafka Log Spam (Patient-service)**
   - **Lỗi**: Consumer spam logs với error messages liên tục
   - **Đã sửa**: ✅ Giảm log frequency và chỉ log critical errors

### ⚠️ **Vấn Đề Nhỏ:**

4. **Kafka Port Inconsistency**
   - **Đã sửa**: ✅ Tất cả default values dùng `localhost:29092` cho local development

5. **Consul Configuration**
   - **Đã sửa**: ✅ Đổi từ `consul:8500` → `localhost:8500` cho local development

---

## 🔧 Chi Tiết Các Thay Đổi

### 1. iam-service - Database Connection ✅

**File**: `iam-service/Program.cs`

**Thay đổi:**
```csharp
// ✅ Auto-detect database host
var dbHost = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
    ? "postgres"  // Docker
    : "localhost"; // Local

// ✅ Replace host in connection string
var connectionString = defaultConnectionString
    .Replace("Host=postgres", $"Host={dbHost}")
    .Replace("Host=localhost", $"Host={dbHost}");
```

**File**: `iam-service/appsettings.json`

**Thay đổi:**
```json
// ✅ Changed from Docker hostname to localhost
"DefaultConnection": "Host=localhost;Port=5432;Database=UserManagementDB;Username=postgres;Password=12345"
```

### 2. Patient-service - Database Connection ✅

**File**: `Patient-service/Program.cs`

**Thay đổi:**
```csharp
// ✅ Auto-detect database server
var dbServer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
    ? "sqlserver"  // Docker service name
    : "localhost"; // Local

// ✅ Replace server in connection string
var connectionString = defaultConnectionString
    .Replace("Server=sqlserver", $"Server={dbServer}")
    .Replace("Server=localhost", $"Server={dbServer}");
```

**File**: `Patient-service/appsettings.json`

**Thay đổi:**
```json
// ✅ Changed from PostgreSQL format to SQL Server format
"DefaultConnection": "Server=localhost;Database=PatientService;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
```

### 3. Kafka Consumer - Log Spam Reduction ✅

**File**: `Patient-service/Events/Consumers/KafkaConsumerService.cs`

**Thay đổi:**
```csharp
// ✅ Improved error handler - only log critical errors
.SetErrorHandler((_, e) => 
{
    // Only log critical errors, suppress connection errors during retry
    if (e.IsFatal || e.Code == ErrorCode.Local_AllBrokersDown)
    {
        _logger.LogWarning("⚠️ Kafka consumer error: {Reason} (Code: {Code})", e.Reason, e.Code);
    }
})

// ✅ Reduced log frequency during retry
if (attempt % 2 == 1 || attempt == 1)
{
    _logger.LogWarning("⚠️ Attempt {Attempt}/{Max}...");
}
```

### 4. Kafka Configuration - Port Consistency ✅

**Files**: 
- `iam-service/Configurations/KafkaConfigExtensions.cs`
- `Patient-service/Configurations/KafkaConfigExtensions.cs`
- `iam-service/Events/Producers/KafkaProducerService.cs`
- `Patient-service/Events/Consumers/KafkaConsumerService.cs`

**Thay đổi:**
```csharp
// ✅ Consistent default: localhost:29092 for local development
(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
    ? "kafka:9092"      // Docker
    : "localhost:29092") // Local
```

### 5. Consul Configuration ✅

**File**: `iam-service/appsettings.json`

**Thay đổi:**
```json
// ✅ Changed from Docker hostname to localhost
"ConsulConfig": {
    "Address": "http://localhost:8500",  // Was: "http://consul:8500"
    "ServiceHost": "localhost"            // Was: "iam-service"
}
```

---

## ✅ Kết Quả Sau Khi Sửa

### iam-service Logs (Expected):
```
✅ [Kafka] Topic exists: iam-service.user-created
✅ [IAMService] Kafka producer configured on localhost:29092
✅ [Consul] Service registered successfully - REST: localhost:7020, gRPC: localhost:7021
Now listening on: http://[::]:7020
Now listening on: http://[::]:7021
Application started. Press Ctrl+C to shut down.
```

### Patient-service Logs (Expected):
```
🔹 [KafkaConsumer] BootstrapServers: localhost:29092, Topic: iam-service.user-created, GroupId: patient-service-consumer
🟢 Kafka consumer started. Listening on topic: iam-service.user-created
Now listening on: http://[::]:7263
Application started. Press Ctrl+C to shut down.
```

### Khi Dependencies Chưa Chạy (Expected):
```
⚠️ [Kafka] Kafka broker is not available at localhost:29092
ℹ️ [Kafka] Service is running normally and will continue to operate.
ℹ️ [Kafka] To start Kafka:
ℹ️ [Kafka]   1. If using Docker: docker-compose up -d kafka zookeeper
ℹ️ [Kafka]   2. Or start Kafka standalone
ℹ️ [Kafka] Topic 'iam-service.user-created' will be auto-created when Kafka is available.
```

---

## 🎯 Next Steps

1. **Start Dependencies:**
   ```bash
   .\start-dependencies.bat
   ```

2. **Verify:**
   ```bash
   .\check-dependencies.bat
   ```

3. **Start Services:**
   - Start iam-service
   - Start Patient-service

4. **Test:**
   - Create user via iam-service API
   - Check Patient-service logs for Kafka message
   - Test gRPC call

---

## 📝 Notes

- **Database**: 
  - iam-service: PostgreSQL (localhost:5432)
  - Patient-service: SQL Server (localhost)
  
- **Kafka**: 
  - Local: localhost:29092
  - Docker: kafka:9092

- **Consul**: 
  - Local: localhost:8500
  - Docker: consul:8500

- **Services vẫn hoạt động** ngay cả khi dependencies chưa sẵn sàng
- **Auto-retry** sẽ tự động kết nối khi dependencies available




















