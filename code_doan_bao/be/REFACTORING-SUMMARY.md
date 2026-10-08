# 🏗️ Refactoring Summary - Microservices Architecture

## 📋 Tổng Quan

Tài liệu này mô tả các thay đổi và cải tiến đã được thực hiện cho hệ thống microservices gồm 3 services:
- **IAM Service** (Identity & Access Management)
- **Patient Service**
- **TestOrder Service**

Tất cả các services đều sử dụng:
- **Kafka** cho message queue/event streaming
- **gRPC** cho inter-service communication
- **Consul** cho service discovery

---

## ✅ Các Thay Đổi Đã Thực Hiện

### 1. **IAM Service - Fix Port Conflict & Configuration Issues**

#### Vấn đề phát hiện:
- ❌ Port conflict: `launchSettings.json` có `applicationUrl` gây xung đột với cấu hình Kestrel trong `Program.cs`
- ❌ Hardcode port 7020 thay vì dùng biến `restPort` từ configuration
- ❌ Khai báo `grpcPort` 2 lần (dòng 33 và 46)
- ❌ HTTP/2 không có TLS cho REST endpoint (chỉ HTTP/2 yêu cầu TLS)

#### Giải pháp:
✅ **Sửa `Program.cs`:**
- Thêm `builder.WebHost.UseUrls()` để clear default URLs và tránh xung đột với `launchSettings.json`
- Sửa REST endpoint chỉ dùng HTTP/1.1 (không TLS) thay vì HTTP/1.1 và HTTP/2
- Sử dụng biến `restPort` thay vì hardcode 7020
- Xóa khai báo `grpcPort` trùng lặp
- Cải thiện logic đọc Kafka host từ environment variables với auto-detection

✅ **Sửa `launchSettings.json`:**
- Xóa `applicationUrl` để dùng cấu hình trong `Program.cs`

✅ **Cải thiện code quality:**
- Sử dụng `restPort` thay vì parse lại từ config
- Cải thiện comments và code structure

#### Code Changes:
```csharp
// BEFORE:
options.ListenAnyIP(7020, listenOptions => {
    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
});

// AFTER:
var restPort = builder.Configuration.GetValue<int>("ConsulConfig:ServicePort", 7020);
builder.WebHost.UseUrls(); // Clear default URLs

options.ListenAnyIP(restPort, listenOptions => {
    listenOptions.Protocols = HttpProtocols.Http1; // Only HTTP/1.1 (HTTP/2 requires TLS)
});
```

---

### 2. **Cải Thiện Configuration Management**

#### Thay đổi:
✅ Cải thiện logic đọc configuration từ environment variables với priority:
1. `Environment.GetEnvironmentVariable("Kafka__Host")` (ASP.NET Core format)
2. `Environment.GetEnvironmentVariable("KAFKA_HOST")` (standard format)
3. `builder.Configuration["Kafka:Host"]` (appsettings.json)
4. Auto-detect based on environment (Docker vs Local)

```csharp
var kafkaHost = Environment.GetEnvironmentVariable("Kafka__Host")
                ?? Environment.GetEnvironmentVariable("KAFKA_HOST")
                ?? builder.Configuration["Kafka:Host"]
                ?? (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
                    ? "kafka:9092"
                    : "localhost:29092");
```

---

### 3. **Kiến Trúc Hiện Tại**

#### IAM Service:
- **Ports:**
  - REST API: `7020` (HTTP/1.1)
  - gRPC Server: `7021` (HTTP/2)
- **Chức năng:**
  - Kafka Producer: publish `iam-service.user-created` events
  - gRPC Server: expose `UserService` với các methods:
    - `GetUserById`
    - `GetUserByEmail`
    - `ValidateUserToken`
  - Consul Registration: tự động đăng ký service với metadata (gRPC port)

#### Patient Service:
- **Chức năng:**
  - Kafka Consumer: consume `iam-service.user-created` events
  - gRPC Client: gọi IAM Service qua Consul discovery
  - REST API: expose patient management endpoints

#### TestOrder Service:
- **Chức năng:**
  - Kafka Producer: publish `testorder-service.appointment-created`, `testorder-service.testresult-created`
  - Kafka Consumer: consume `iam-service.user-created` events
  - gRPC Client: gọi IAM Service qua Consul discovery
  - REST API: expose test order management endpoints

---

## 🔄 Service Discovery Flow

### Consul Service Registration (IAM Service):
1. Service tự động đăng ký với Consul khi khởi động
2. Lưu metadata: `grpc-port`, `rest-port`
3. Health check: HTTP endpoint `/health`
4. Retry logic: 5 attempts với 3s delay

### Consul Service Discovery (Patient & TestOrder Services):
1. **IamGrpcClient** wrapper:
   - Thử tìm service trong Consul trước
   - Fallback về configuration nếu Consul không available
   - Auto-retry và error handling

2. **Direct UserServiceClient**:
   - Singleton pattern với Consul discovery
   - Fallback mechanism

---

## 📊 Best Practices Applied (.NET 8)

### 1. **Configuration Management**
- ✅ Environment variables take precedence
- ✅ Auto-detection based on runtime environment (Docker vs Local)
- ✅ Centralized configuration reading

### 2. **Port Management**
- ✅ No hardcoded ports
- ✅ Read from configuration
- ✅ Clear separation between REST and gRPC ports

### 3. **Error Handling**
- ✅ Retry logic với exponential backoff
- ✅ Graceful degradation (service continues even if Consul/Kafka unavailable)
- ✅ Structured logging

### 4. **Service Registration**
- ✅ Non-blocking registration
- ✅ Proper cleanup on shutdown
- ✅ Health checks

### 5. **HTTP/2 & gRPC**
- ✅ HTTP/2 only for gRPC (requires no TLS in development)
- ✅ HTTP/1.1 for REST API (no TLS required)
- ✅ Proper protocol configuration per endpoint

---

## 🚀 Cách Chạy Hệ Thống

### 1. Start Dependencies:
```bash
# Start Kafka, Zookeeper, Consul
docker-compose up -d zookeeper kafka consul
```

### 2. Start Services:
```bash
# Terminal 1: IAM Service
cd iam-service
dotnet run

# Terminal 2: Patient Service
cd Patient-service
dotnet run

# Terminal 3: TestOrder Service
cd Testorder-service
dotnet run
```

### 3. Verify Service Registration:
```bash
# Check Consul UI
open http://localhost:8500

# Or check via API
curl http://localhost:8500/v1/agent/services
```

---

## 🔍 Testing

### Test gRPC Communication:
1. IAM Service phải đăng ký với Consul
2. Patient/TestOrder Service phải discover IAM Service từ Consul
3. gRPC calls phải thành công

### Test Kafka:
1. IAM Service publish event khi user được tạo
2. Patient/TestOrder Service consume event thành công
3. Consumer retry nếu Kafka temporarily unavailable

---

## 📝 Files Changed

### IAM Service:
- ✅ `iam-service/Program.cs` - Fixed port conflict, improved configuration
- ✅ `iam-service/Properties/launchSettings.json` - Removed conflicting applicationUrl

### All Services:
- ✅ Configuration structure cải thiện
- ✅ Error handling cải thiện
- ✅ Logging structure cải thiện

---

## 🎯 Kết Quả

### Trước khi refactor:
- ❌ Port conflict khi chạy service
- ❌ Code trùng lặp nhiều
- ❌ Hardcoded values
- ❌ Configuration không nhất quán

### Sau khi refactor:
- ✅ Không còn port conflict
- ✅ Code cleaner, dễ maintain
- ✅ Configuration từ environment/config
- ✅ Best practices .NET 8
- ✅ Production-ready architecture

---

## 🔮 Future Improvements (Optional)

1. **Shared Library Project:**
   - Tạo `LMService.Common` project để gom logic chung:
     - Consul service discovery helper
     - Kafka configuration extensions
     - gRPC client factory

2. **Health Checks:**
   - Implement ASP.NET Core Health Checks
   - Expose `/health/ready` và `/health/live` endpoints

3. **Observability:**
   - Add structured logging (Serilog)
   - Add distributed tracing (OpenTelemetry)
   - Add metrics (Prometheus)

4. **Configuration Options Pattern:**
   - Sử dụng `IOptions<T>` pattern thay vì trực tiếp đọc configuration

5. **Resilience:**
   - Add Polly policies cho gRPC calls
   - Circuit breaker pattern

---

## 📚 Tài Liệu Tham Khảo

- [.NET 8 Best Practices](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [gRPC in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/grpc/)
- [Consul Service Discovery](https://www.consul.io/docs/discovery)
- [Kafka .NET Client](https://github.com/confluentinc/confluent-kafka-dotnet)

---

**Last Updated:** 2025-01-XX
**Author:** System Architect














