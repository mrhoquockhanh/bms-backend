# 🚀 Quick Fix: Kafka Connection Error

## ⚠️ **LỖI BẠN ĐANG THẤY LÀ BÌNH THƯỜNG!**

```
%3|...|FAIL|rdkafka#producer-1| Connect to ipv4#127.0.0.1:29092 failed
%3|...|ERROR|rdkafka#producer-1| 1/1 brokers are down
```

**→ Đây KHÔNG PHẢI lỗi nghiêm trọng!**
- ✅ Service đã start thành công
- ✅ Tất cả components đã được cấu hình đúng
- ❌ Chỉ là Kafka broker chưa chạy

---

## ✅ **GIẢI PHÁP ĐƠN GIẢN**

### **Cách 1: Dùng Script (Khuyên dùng)**

```bash
# Windows Batch
.\start-kafka.bat

# PowerShell
.\start-kafka.ps1
```

### **Cách 2: Docker Compose Trực Tiếp**

```bash
docker-compose up -d zookeeper kafka
```

### **Cách 3: Kiểm Tra và Start Thủ Công**

```bash
# 1. Check xem Kafka đã chạy chưa
docker ps | findstr kafka

# 2. Nếu chưa có, start Kafka
docker-compose up -d zookeeper kafka

# 3. Đợi 10-15 giây cho Kafka fully start
timeout /t 15

# 4. Verify
docker logs kafka --tail 20
```

---

## ✅ **SAU KHI START KAFKA**

Bạn sẽ thấy logs thay đổi:

### **Trước (Khi Kafka chưa chạy):**
```
%3|...|ERROR|rdkafka#producer-1| 1/1 brokers are down
⚠️ [Kafka] Attempt 1/3: Failed to get metadata
```

### **Sau (Khi Kafka đã chạy):**
```
✅ [Kafka] Topic exists: testorder-service.appointment-created
✅ [Kafka] Topic exists: testorder-service.testresult-created
✅ [TestOrderService] Kafka topics configured. Service is ready.
🟢 Kafka consumer started. Listening on topics: iam-service.user-created
```

**→ Không còn lỗi!**

---

## 🔍 **VERIFY KAFKA ĐANG CHẠY**

### **Check Containers:**
```bash
docker ps | findstr kafka
```

**Expected output:**
```
CONTAINER ID   IMAGE                          STATUS
xxxxx          confluentinc/cp-kafka:7.5.0   Up X minutes  0.0.0.0:9092->9092/tcp, 0.0.0.0:29092->29092/tcp
```

### **Test Connection:**
```powershell
# PowerShell
Test-NetConnection localhost -Port 29092

# Expected:
# TcpTestSucceeded : True
```

### **Check Logs:**
```bash
docker logs kafka --tail 20
```

**Expected:**
```
[2024-...] INFO Kafka Server started (kafka.server.KafkaServer)
```

---

## 📝 **TẠI SAO CÓ LỖI?**

### **1. Service Start Trước Kafka:**
- Service khởi động và cố gắng kết nối Kafka
- Kafka chưa chạy → Connection failed
- **→ Đây là BÌNH THƯỜNG!**

### **2. Logs từ rdkafka:**
- `%3|...` là logs từ thư viện C (librdkafka)
- Không thể suppress hoàn toàn từ C# code
- **→ Đây là expected behavior**

### **3. Service Vẫn Hoạt Động:**
- ✅ REST API endpoints
- ✅ gRPC Client
- ✅ Database
- ✅ Business logic
- ❌ Chỉ Kafka Producer/Consumer chưa kết nối

---

## 🎯 **BEST PRACTICE**

### **Development:**
1. **Start Kafka trước:**
   ```bash
   .\start-kafka.bat
   ```

2. **Sau đó start services:**
   ```bash
   cd iam-service
   dotnet run
   
   cd ../Patient-service
   dotnet run
   
   cd ../Testorder-service
   dotnet run
   ```

### **Hoặc Start Tất Cả Cùng Lúc:**
```bash
# Terminal 1: Dependencies
.\start-kafka.bat

# Terminal 2: iam-service
cd iam-service && dotnet run

# Terminal 3: Patient-service
cd Patient-service && dotnet run

# Terminal 4: Testorder-service
cd Testorder-service && dotnet run
```

---

## ✅ **TÓM TẮT**

| Vấn Đề | Trạng Thái | Hành Động |
|--------|-----------|-----------|
| Service start | ✅ OK | Không cần làm gì |
| Kafka connection | ⚠️ Pending | **Start Kafka** |
| Logs `%3|...` | ℹ️ Normal | Bình thường |
| Service functionality | ✅ OK | Vẫn hoạt động |

**→ CHỈ CẦN START KAFKA LÀ XONG!** 🎉

---

## 🚀 **QUICK COMMAND**

```bash
# Start Kafka
.\start-kafka.bat

# Hoặc
docker-compose up -d zookeeper kafka

# Verify
docker ps | findstr kafka
Test-NetConnection localhost -Port 29092
```

**Sau đó restart service và lỗi sẽ biến mất!** ✅



















