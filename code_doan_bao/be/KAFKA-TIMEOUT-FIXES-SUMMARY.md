# ✅ KAFKA TIMEOUT FIXES - TÓM TẮT

## 🔧 **CÁC THAY ĐỔI ĐÃ ÁP DỤNG**

### **1. AdminClient Configuration** (Topic Creation)
**Files:**
- `iam-service/Configurations/KafkaConfigExtensions.cs`
- `Testorder-service/Configurations/KafkaConfigExtensions.cs`

**Changes:**
```csharp
// BEFORE
SocketTimeoutMs = 5000  // 5 seconds

// AFTER
SocketTimeoutMs = 30000,        // 30 seconds (increased for Docker)
RequestTimeoutMs = 30000,        // 30 seconds (NEW)
MetadataRequestTimeoutMs = 30000 // 30 seconds (NEW)
```

**Retry Logic:**
```csharp
// BEFORE
const int maxRetries = 3;
const int retryDelayMs = 2000;

// AFTER
const int maxRetries = 5;           // Increased from 3
const int retryDelayMs = 5000;      // Increased from 2000ms
```

---

### **2. Producer Configuration**
**Files:**
- `iam-service/Events/Producers/KafkaProducerService.cs`
- `Testorder-service/Events/Producers/KafkaProducerService.cs`

**Changes:**
```csharp
// BEFORE
RetryBackoffMs = 100,
MessageSendMaxRetries = 3

// AFTER
RetryBackoffMs = 1000,          // Increased from 100ms
MessageSendMaxRetries = 5,      // Increased from 3
RequestTimeoutMs = 30000,        // 30 seconds (NEW - for Docker)
SocketTimeoutMs = 30000          // 30 seconds (NEW - for Docker)
```

---

### **3. Consumer Configuration**
**Files:**
- `Patient-service/Events/Consumers/KafkaConsumerService.cs`
- `Testorder-service/Events/Consumers/KafkaConsumerService.cs`

**Changes:**
```csharp
// BEFORE
SocketTimeoutMs = 5000,
SessionTimeoutMs = 10000

// AFTER
SocketTimeoutMs = 30000,        // 30 seconds (increased for Docker)
SessionTimeoutMs = 30000,       // 30 seconds (increased for Docker)
RequestTimeoutMs = 30000,        // 30 seconds (NEW)
MetadataMaxAgeMs = 300000        // 5 minutes (NEW)
```

---

## 📊 **SO SÁNH TRƯỚC/SAU**

| Component | Setting | Trước | Sau | Lý do |
|-----------|---------|-------|-----|-------|
| **AdminClient** | SocketTimeoutMs | 5s | 30s | Docker network latency |
| **AdminClient** | RequestTimeoutMs | ❌ | 30s | Missing timeout |
| **AdminClient** | MetadataRequestTimeoutMs | ❌ | 30s | Missing timeout |
| **AdminClient** | Max Retries | 3 | 5 | More attempts |
| **AdminClient** | Retry Delay | 2s | 5s | More time between retries |
| **Producer** | RetryBackoffMs | 100ms | 1000ms | More backoff time |
| **Producer** | Max Retries | 3 | 5 | More attempts |
| **Producer** | RequestTimeoutMs | ❌ | 30s | Missing timeout |
| **Producer** | SocketTimeoutMs | ❌ | 30s | Missing timeout |
| **Consumer** | SocketTimeoutMs | 5s | 30s | Docker network latency |
| **Consumer** | SessionTimeoutMs | 10s | 30s | More time for session |
| **Consumer** | RequestTimeoutMs | ❌ | 30s | Missing timeout |
| **Consumer** | MetadataMaxAgeMs | ❌ | 5min | Cache metadata longer |

---

## ✅ **KẾT QUẢ MONG ĐỢI**

### **Trước khi sửa:**
```
❌ Timed out CreateTopicsRequest in flight (after 5019ms)
❌ Timed out FetchRequest in flight (after 5571ms)
```

### **Sau khi sửa:**
```
✅ Topics created successfully
✅ Consumer connected successfully
✅ No timeout errors
```

---

## 🚀 **TESTING**

Sau khi rebuild và restart services:

```bash
# Rebuild services
docker-compose up -d --build

# Check logs
docker-compose logs -f iam-service
docker-compose logs -f testorder-service
docker-compose logs -f patient-service

# Verify Kafka topics
docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list
```

---

## 📝 **LƯU Ý**

1. **Timeout 30 giây** là phù hợp cho Docker environment
2. **Retry 5 lần** với delay 5 giây = tối đa 25 giây retry
3. **MetadataMaxAgeMs = 5 phút** giúp giảm số lần query metadata
4. Các services sẽ **tự động retry** nếu Kafka chưa sẵn sàng

---

## 🎯 **KẾT LUẬN**

Tất cả timeout configurations đã được tăng lên **30 giây** và thêm các timeout settings cần thiết để phù hợp với Docker environment. Hệ thống sẽ **ổn định hơn** và **ít bị timeout** hơn.

