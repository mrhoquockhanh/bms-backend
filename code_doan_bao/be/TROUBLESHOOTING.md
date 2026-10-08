# Troubleshooting Guide - Kafka & gRPC Integration

## Phân tích Log và Các Vấn Đề Tiềm Ẩn

### 📊 Phân tích Log Hiện Tại

Từ log bạn cung cấp, tôi đã phát hiện các vấn đề sau:

#### ✅ **Những gì ĐANG HOẠT ĐỘNG ĐÚNG:**

1. **Service Startup**: ✅
   ```
   ✅ Application started
   ✅ Now listening on: http://[::]:7020 (REST API)
   ✅ Now listening on: http://[::]:7021 (gRPC)
   ```

2. **Kafka Configuration**: ✅
   - Đã detect đúng `localhost:29092`
   - Retry logic hoạt động (3 attempts)
   - Service không bị crash khi Kafka unavailable

3. **Error Handling**: ✅
   - Graceful fallback khi dependencies unavailable
   - Logging rõ ràng với hướng dẫn

#### ⚠️ **Các Vấn Đề Tiềm Ẩn ĐÃ ĐƯỢC SỬA:**

1. **Kafka Port Inconsistency** ✅ FIXED
   - **Vấn đề**: `KafkaConfigExtensions.cs` dùng `localhost:9092` thay vì `localhost:29092`
   - **Đã sửa**: Tất cả default values đã được cập nhật thành `localhost:29092`

2. **Environment Variable Detection** ✅ IMPROVED
   - **Vấn đề**: Không nhất quán giữa các service
   - **Đã sửa**: Đồng bộ detection logic giữa cả 2 service

3. **Consul Configuration** ✅ FIXED
   - **Vấn đề**: iam-service dùng `consul:8500` (chỉ work trong Docker)
   - **Đã sửa**: Đổi thành `localhost:8500` cho local development

---

## 🔍 Các Vấn Đề Tiềm Ẩn Khác

### 1. **Kafka Chưa Được Start**

**Triệu chứng:**
```
Connect to ipv4#127.0.0.1:29092 failed: Unknown error
1/1 brokers are down
```

**Nguyên nhân:**
- Kafka container chưa được start
- Hoặc Kafka đang trong quá trình khởi động

**Giải pháp:**
```bash
# Check status
.\check-dependencies.bat
# hoặc
.\check-dependencies.ps1

# Start Kafka
docker-compose up -d zookeeper kafka

# Wait 10-15 seconds cho Kafka fully start
# Verify
docker logs kafka
```

### 2. **Port Conflict**

**Triệu chứng:**
```
Connect to localhost:29092 failed
```

**Nguyên nhân có thể:**
- Port 29092 đã bị sử dụng bởi process khác
- Docker port mapping không đúng

**Giải pháp:**
```bash
# Check port usage (Windows)
netstat -ano | findstr :29092

# Check port usage (Linux/Mac)
lsof -i :29092
# hoặc
netstat -tulpn | grep 29092

# Nếu port bị chiếm, kill process hoặc đổi port trong docker-compose.yml
```

### 3. **Kafka Listener Configuration**

**Vấn đề tiềm ẩn:**
- `KAFKA_ADVERTISED_LISTENERS` phải match với cách client connect
- Nếu connect từ localhost → dùng `PLAINTEXT_HOST://localhost:29092`
- Nếu connect từ Docker network → dùng `PLAINTEXT://kafka:9092`

**Kiểm tra:**
```bash
# Check Kafka config
docker exec kafka kafka-configs --bootstrap-server localhost:9092 --entity-type brokers --entity-name 1 --describe --all
```

### 4. **Consul Connection Refused**

**Triệu chứng:**
```
No connection could be made because the target machine actively refused it. (localhost:8500)
```

**Nguyên nhân:**
- Consul chưa được start
- Hoặc port 8500 bị block

**Giải pháp:**
```bash
# Start Consul
docker-compose up -d consul

# Verify
curl http://localhost:8500/v1/status/leader
# hoặc mở browser: http://localhost:8500
```

### 5. **gRPC Port Configuration**

**Vấn đề tiềm ẩn:**
- gRPC server listen trên port 7021
- Client phải connect đúng port này
- Consul metadata phải chứa gRPC port

**Đã được xử lý:**
- ✅ IamGrpcClient dùng fixed port 7021
- ✅ Consul registration có metadata `grpc-port: 7021`

---

## 🛠️ Diagnostic Tools

### Script Kiểm Tra Dependencies

```bash
# Windows
.\check-dependencies.bat
.\check-dependencies.ps1

# Linux/Mac
chmod +x check-dependencies.sh
./check-dependencies.sh
```

### Manual Checks

```bash
# 1. Check Docker containers
docker ps --filter "name=kafka" --filter "name=zookeeper" --filter "name=consul"

# 2. Check Kafka logs
docker logs kafka --tail 50

# 3. Test Kafka connection
telnet localhost 29092
# hoặc
nc -zv localhost 29092

# 4. Test Consul
curl http://localhost:8500/v1/status/leader

# 5. Check Kafka topics
docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list
```

---

## 🔧 Quick Fixes

### Fix 1: Start All Dependencies
```bash
.\start-dependencies.bat
```

### Fix 2: Restart Kafka
```bash
docker-compose restart kafka zookeeper
```

### Fix 3: Check Kafka Health
```bash
docker exec kafka kafka-broker-api-versions --bootstrap-server localhost:9092
```

### Fix 4: Verify Port Mapping
```bash
# Check if ports are exposed
docker port kafka
# Should show:
# 9092/tcp -> 0.0.0.0:9092
# 29092/tcp -> 0.0.0.0:29092
```

---

## 📋 Checklist Trước Khi Test

- [ ] Docker đang chạy
- [ ] Zookeeper container đang chạy
- [ ] Kafka container đang chạy và healthy
- [ ] Consul container đang chạy (optional)
- [ ] Port 29092 accessible từ host
- [ ] Port 8500 accessible từ host (nếu dùng Consul)
- [ ] Services đã được build và start
- [ ] appsettings.json có cấu hình đúng

---

## 🎯 Expected Behavior

### Khi TẤT CẢ dependencies đã start:

**iam-service logs:**
```
✅ [Kafka] Topic exists: iam-service.user-created
✅ [IAMService] Kafka producer configured on localhost:29092
✅ [Consul] Service registered successfully - REST: localhost:7020, gRPC: localhost:7021
```

**Patient-service logs:**
```
🟢 Kafka consumer started. Listening on topic: iam-service.user-created
🔗 Discovered IAMService in Consul: localhost
```

### Khi dependencies CHƯA start (bình thường):

**iam-service logs:**
```
⚠️ [Kafka] Kafka broker is not available at localhost:29092
ℹ️ [Kafka] Service is running normally and will continue to operate.
⚠️ [Consul] Failed to register after 5 attempts. Service will continue without Consul registration.
```

**Patient-service logs:**
```
⚠️ Attempt 1/5 failed to connect to Kafka: ... Retrying in 3000ms...
```

---

## 🚀 Next Steps

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

4. **Test Integration:**
   - Create user via iam-service API
   - Check Patient-service logs for Kafka message
   - Test gRPC call from Patient-service

---

## 📞 Common Issues & Solutions

| Issue | Symptom | Solution |
|-------|---------|----------|
| Kafka not accessible | `Connect to localhost:29092 failed` | Start Kafka: `docker-compose up -d kafka zookeeper` |
| Port already in use | `Address already in use` | Check port: `netstat -ano \| findstr :29092` |
| Consul not found | `No such host is known` | Start Consul: `docker-compose up -d consul` |
| Topic not created | No topic in Kafka | Wait for auto-create or create manually |
| gRPC connection failed | `Connection refused` | Check iam-service is running on port 7021 |

---

## ✅ Verification Commands

```bash
# 1. Check all dependencies
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | findstr "kafka zookeeper consul"

# 2. Test Kafka
docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list

# 3. Test Consul
curl http://localhost:8500/v1/agent/services

# 4. Check service health
curl http://localhost:7020/health  # iam-service
curl http://localhost:7263/         # patient-service
```

---

## 📝 Notes

- **Service vẫn hoạt động** ngay cả khi Kafka/Consul chưa sẵn sàng
- **Auto-retry** sẽ tự động kết nối khi dependencies available
- **Topic auto-create** khi publish message đầu tiên (nếu `KAFKA_AUTO_CREATE_TOPICS_ENABLE: "true"`)
- **gRPC fallback** sử dụng config nếu Consul unavailable




















