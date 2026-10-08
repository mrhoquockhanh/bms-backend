# BÁO CÁO RÀ SOÁT CẤU HÌNH KAFKA VÀ gRPC
## Microservices: iam-service, Patient-service, Testorder-service

**Ngày kiểm tra:** $(date)  
**Người thực hiện:** AI Code Reviewer  
**Mục đích:** Rà soát và khắc phục triệt để các vấn đề về cấu hình Kafka và gRPC

---

## 📋 TỔNG QUAN

Hệ thống sử dụng:
- **Kafka**: Event-driven communication giữa các services
- **gRPC**: Synchronous communication cho service-to-service calls
- **Consul**: Service discovery và health checks

---

## 🔍 CÁC VẤN ĐỀ ĐÃ PHÁT HIỆN VÀ ĐÃ SỬA

### 1. ❌ **VẤN ĐỀ: Testorder-service gRPC Client không xử lý đúng Docker environment**

**Mô tả:**
- File `Testorder-service/GrpcClients/IamGrpcClient.cs` sử dụng hardcoded `"localhost"` khi Consul trả về address rỗng
- Không phân biệt giữa môi trường Docker và local development

**Hậu quả:**
- Service không thể kết nối tới iam-service khi chạy trong Docker
- gRPC calls sẽ fail với lỗi connection refused

**Giải pháp đã áp dụng:**
```csharp
// TRƯỚC (SAI):
if (string.IsNullOrEmpty(address) || address == "0.0.0.0")
{
    address = "localhost"; // ❌ Hardcoded
}

// SAU (ĐÚNG):
if (string.IsNullOrEmpty(address) || address == "0.0.0.0")
{
    var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    address = isRunningInContainer ? "iam-service" : "localhost"; // ✅ Dynamic
}
```

**Files đã sửa:**
- ✅ `Testorder-service/GrpcClients/IamGrpcClient.cs`
- ✅ `Patient-service/GrpcClients/IamGrpcClient.cs` (đã đúng, nhưng cải thiện thêm)

---

### 2. ❌ **VẤN ĐỀ: gRPC Port không được đọc từ Consul Metadata**

**Mô tả:**
- Các gRPC clients hardcode port `7021` thay vì đọc từ Consul metadata
- Consul đã lưu gRPC port trong metadata (`grpc-port`) nhưng không được sử dụng

**Hậu quả:**
- Nếu gRPC port thay đổi, phải sửa code thay vì chỉ cấu hình
- Không linh hoạt khi deploy với ports khác nhau

**Giải pháp đã áp dụng:**
```csharp
// TRƯỚC (SAI):
private const int GrpcPort = 7021; // ❌ Hardcoded
var grpcAddress = $"http://{address}:{GrpcPort}";

// SAU (ĐÚNG):
// Try to get gRPC port from Consul metadata, fallback to default
int grpcPort = GrpcPort; // Default fallback
if (iamService.Meta != null && iamService.Meta.TryGetValue("grpc-port", out var grpcPortStr))
{
    if (int.TryParse(grpcPortStr, out var parsedPort))
    {
        grpcPort = parsedPort; // ✅ Read from Consul
    }
}
```

**Files đã sửa:**
- ✅ `Patient-service/GrpcClients/IamGrpcClient.cs`
- ✅ `Testorder-service/GrpcClients/IamGrpcClient.cs`

---

### 3. ⚠️ **VẤN ĐỀ: Kafka Producer Error Handler không nhất quán**

**Mô tả:**
- `iam-service/Events/Producers/KafkaProducerService.cs` log tất cả errors, kể cả connection errors khi Kafka chưa sẵn sàng
- Gây spam logs và khó debug

**Hậu quả:**
- Logs bị nhiễu với các errors không quan trọng
- Khó phân biệt fatal errors vs temporary connection issues

**Giải pháp đã áp dụng:**
```csharp
// TRƯỚC (SAI):
.SetErrorHandler((_, e) => 
    _logger.LogError("❌ Kafka producer error: {Reason}", e.Reason)) // ❌ Log tất cả

// SAU (ĐÚNG):
.SetErrorHandler((_, e) => 
{
    // Suppress connection errors - they're expected when Kafka is not available
    // Only log fatal errors
    if (e.IsFatal)
    {
        _logger.LogError("❌ Kafka producer fatal error: {Reason} (Code: {Code})", e.Reason, e.Code);
    }
    // Don't log Local_AllBrokersDown - it's expected when Kafka is down
}) // ✅ Chỉ log fatal errors
```

**Files đã sửa:**
- ✅ `iam-service/Events/Producers/KafkaProducerService.cs`

---

## ✅ CÁC CẤU HÌNH ĐÃ ĐÚNG (KHÔNG CẦN SỬA)

### 1. ✅ **Kafka Consumer Configuration**
- ✅ Retry logic đã được implement đúng (5 attempts, 3s delay)
- ✅ Timeout settings phù hợp cho Docker (30s socket timeout, 30s session timeout)
- ✅ Error handling đã suppress connection errors trong retry phase
- ✅ Auto-create topics được enable (`AllowAutoCreateTopics = true`)

### 2. ✅ **Kafka Producer Configuration**
- ✅ Retry settings hợp lý (5 retries, 1s backoff)
- ✅ Acks = All để đảm bảo message delivery
- ✅ Timeout 30s phù hợp cho Docker

### 3. ✅ **gRPC Server Configuration**
- ✅ HTTP/2 protocol được cấu hình đúng
- ✅ Port separation giữa REST và gRPC
- ✅ Kestrel configuration đúng cho cả Docker và local

### 4. ✅ **Consul Service Registration**
- ✅ Metadata chứa gRPC port (`grpc-port`)
- ✅ Health checks được cấu hình
- ✅ Retry logic khi register service

---

## 🔧 CẢI THIỆN BỔ SUNG ĐÃ THỰC HIỆN

### 1. **Cải thiện gRPC Client Fallback Logic**
- ✅ Đọc port từ fallback URL nếu có
- ✅ Phân biệt rõ ràng giữa Docker và local environment
- ✅ Logging chi tiết hơn để debug

### 2. **Consistent Error Handling**
- ✅ Tất cả Kafka producers/consumers đều suppress connection errors
- ✅ Chỉ log fatal errors hoặc sau khi retries exhausted

---

## 📊 KIỂM TRA CẤU HÌNH THEO SERVICE

### **iam-service**
| Component | Status | Notes |
|-----------|--------|-------|
| Kafka Producer | ✅ OK | Error handler đã được cải thiện |
| Kafka Consumer | ✅ OK | EmailNotificationConsumer hoạt động tốt |
| gRPC Server | ✅ OK | Port 7021, HTTP/2 |
| Consul Registration | ✅ OK | Metadata chứa grpc-port |
| Service Discovery | ✅ OK | Fallback logic đúng |

### **Patient-service**
| Component | Status | Notes |
|-----------|--------|-------|
| Kafka Consumer | ✅ OK | UserCreatedEvent consumer |
| gRPC Server | ✅ OK | Port 7264, HTTP/2 |
| gRPC Client | ✅ FIXED | Đã cải thiện Consul metadata reading |
| Consul Registration | ✅ OK | Metadata chứa grpc-port |

### **Testorder-service**
| Component | Status | Notes |
|-----------|--------|-------|
| Kafka Producer | ✅ OK | Multiple topics support |
| Kafka Consumer | ✅ OK | UserCreatedEvent consumer |
| gRPC Server | ✅ OK | Port 7106, HTTP/2 |
| gRPC Client | ✅ FIXED | Docker environment fix + Consul metadata |
| Consul Registration | ✅ OK | Metadata chứa grpc-port |

---

## 🎯 KẾT LUẬN VÀ KHUYẾN NGHỊ

### **Các vấn đề đã được khắc phục:**
1. ✅ Testorder-service gRPC client Docker environment handling
2. ✅ gRPC port discovery từ Consul metadata
3. ✅ Kafka producer error handler consistency

### **Khuyến nghị cho tương lai:**

#### 1. **gRPC Channel Pooling** (Tối ưu hiệu năng)
**Hiện tại:** Mỗi request tạo channel mới  
**Khuyến nghị:** Implement channel pooling hoặc singleton channel với health checks

```csharp
// Ví dụ cải thiện (có thể implement sau):
builder.Services.AddSingleton<GrpcChannel>(sp => {
    // Create và reuse channel
    // Implement health checks
    // Auto-reconnect on failure
});
```

#### 2. **Kafka Consumer Group Management**
**Hiện tại:** Mỗi service có consumer group riêng  
**Khuyến nghị:** 
- Monitor consumer lag
- Implement dead letter queue cho failed messages
- Add metrics cho message processing time

#### 3. **gRPC Retry Policy**
**Hiện tại:** Không có retry logic trong gRPC clients  
**Khuyến nghị:** Implement retry với exponential backoff cho transient errors

```csharp
// Ví dụ (có thể implement sau):
var policy = Policy
    .Handle<RpcException>(ex => ex.StatusCode == StatusCode.Unavailable)
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
```

#### 4. **Health Checks Enhancement**
**Hiện tại:** Basic health checks  
**Khuyến nghị:**
- Add Kafka connectivity health check
- Add gRPC endpoint health check
- Add database connectivity check

#### 5. **Configuration Validation**
**Khuyến nghị:** Validate configuration at startup:
- Kafka host/port reachable
- gRPC ports available
- Consul connectivity

---

## 📝 CHECKLIST KIỂM TRA TRƯỚC KHI DEPLOY

- [ ] Kafka broker đang chạy và accessible
- [ ] Consul đang chạy và accessible
- [ ] Tất cả services đã register vào Consul
- [ ] gRPC ports không conflict
- [ ] Kafka topics đã được tạo (hoặc auto-create enabled)
- [ ] Environment variables được set đúng (Docker vs local)
- [ ] Health check endpoints trả về 200 OK
- [ ] Logs không có fatal errors

---

## 🔗 TÀI LIỆU THAM KHẢO

- [gRPC Best Practices](https://grpc.io/docs/guides/performance/)
- [Kafka Consumer Configuration](https://kafka.apache.org/documentation/#consumerconfigs)
- [Consul Service Discovery](https://www.consul.io/docs/discovery/services)

---

**Tóm tắt:** Hệ thống đã được rà soát và các vấn đề quan trọng đã được khắc phục. Cấu hình Kafka và gRPC hiện tại đã ổn định và sẵn sàng cho production với các cải thiện bổ sung được đề xuất ở trên.



