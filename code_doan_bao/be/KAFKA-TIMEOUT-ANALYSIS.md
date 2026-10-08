# 🔍 PHÂN TÍCH LỖI KAFKA TIMEOUT

## ❌ **LỖI PHÁT HIỆN**

```
%5|1763101977.139|REQTMOUT|rdkafka#producer-1| [thrd:kafka:9092/1]: 
kafka:9092/1: Timed out CreateTopicsRequest in flight (after 5019ms, timeout #0)

%4|1763101977.180|REQTMOUT|rdkafka#producer-1| [thrd:kafka:9092/1]: 
kafka:9092/1: Timed out 1 in-flight, 0 retry-queued, 0 out-queue, 0 partially-sent requests

⚠️ [Kafka] Attempt 1/3: Error connecting to Kafka: 
Failed while waiting for response from broker: Local: Timed out. Retrying...

%5|1763102056.419|REQTMOUT|rdkafka#consumer-2| [thrd:kafka:9092/1]: 
kafka:9092/1: Timed out FetchRequest in flight (after 5571ms, timeout #0)
```

---

## 🔍 **NGUYÊN NHÂN**

### **1. Timeout Quá Ngắn** ⏱️
- **SocketTimeoutMs = 5000ms** (5 giây) - **QUÁ NGẮN** cho Docker environment
- Kafka trong Docker cần thời gian khởi động và xử lý requests
- Network latency trong Docker network có thể cao hơn

### **2. Kafka Chưa Sẵn Sàng** 🚀
- Services start trước khi Kafka hoàn toàn ready
- Health check chỉ kiểm tra port, không kiểm tra Kafka broker ready
- Metadata requests fail vì broker chưa sẵn sàng

### **3. Thiếu Request Timeout Configuration** ⚙️
- AdminClient chỉ có `SocketTimeoutMs`, thiếu `RequestTimeoutMs`
- Consumer thiếu `RequestTimeoutMs` và `MetadataMaxAgeMs`

---

## ✅ **GIẢI PHÁP**

### **1. Tăng Timeout Values**
- `SocketTimeoutMs`: 5000ms → **30000ms** (30 giây)
- Thêm `RequestTimeoutMs`: **30000ms** (30 giây)
- Thêm `MetadataMaxAgeMs`: **300000ms** (5 phút)

### **2. Thêm Delay Trước Khi Kết Nối**
- Đợi 5-10 giây sau khi service start trước khi kết nối Kafka
- Cho Kafka thời gian khởi động hoàn toàn

### **3. Cải Thiện Retry Logic**
- Tăng số lần retry: 3 → **5 lần**
- Tăng delay giữa các retry: 2000ms → **5000ms**

---

## 🔧 **CÁC FILE CẦN SỬA**

1. ✅ `iam-service/Configurations/KafkaConfigExtensions.cs`
2. ✅ `Testorder-service/Configurations/KafkaConfigExtensions.cs`
3. ✅ `Patient-service/Configurations/KafkaConfigExtensions.cs` (nếu có)
4. ✅ `Testorder-service/Events/Consumers/KafkaConsumerService.cs`
5. ✅ `Patient-service/Events/Consumers/KafkaConsumerService.cs`
6. ✅ `iam-service/Events/Producers/KafkaProducerService.cs`
7. ✅ `Testorder-service/Events/Producers/KafkaProducerService.cs`

---

## 📊 **TIMEOUT CONFIGURATION RECOMMENDED**

### **AdminClient (Topic Creation)**
```csharp
new AdminClientConfig
{
    BootstrapServers = bootstrapServers,
    SocketTimeoutMs = 30000,        // 30 seconds
    RequestTimeoutMs = 30000,        // 30 seconds (NEW)
    MetadataRequestTimeoutMs = 30000 // 30 seconds (NEW)
}
```

### **Consumer**
```csharp
new ConsumerConfig
{
    BootstrapServers = _bootstrapServers,
    GroupId = _groupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true,
    AllowAutoCreateTopics = true,
    SocketTimeoutMs = 30000,        // 30 seconds (increased)
    SessionTimeoutMs = 30000,       // 30 seconds (increased)
    RequestTimeoutMs = 30000,        // 30 seconds (NEW)
    MetadataMaxAgeMs = 300000        // 5 minutes (NEW)
}
```

### **Producer**
```csharp
new ProducerConfig
{
    BootstrapServers = _bootstrapServers,
    Acks = Acks.All,
    RetryBackoffMs = 1000,          // Increased from 100
    MessageSendMaxRetries = 5,      // Increased from 3
    RequestTimeoutMs = 30000,        // 30 seconds (NEW)
    SocketTimeoutMs = 30000          // 30 seconds (NEW)
}
```

---

## 🎯 **KẾT LUẬN**

**Vấn đề chính**: Timeout quá ngắn (5 giây) không đủ cho Docker environment.

**Giải pháp**: Tăng timeout lên 30 giây và thêm các timeout configurations cần thiết.

