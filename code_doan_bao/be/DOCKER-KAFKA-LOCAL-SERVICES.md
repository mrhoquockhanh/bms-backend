# Kafka Docker + Services Local - Có Kết Nối Được Không?

## ✅ **TRẢ LỜI: CÓ, HOÀN TOÀN ĐƯỢC!** 🎉

---

## 📊 Kiến Trúc Hiện Tại

```
┌─────────────────────────────────────────────────────────┐
│                    Docker Network                      │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐         │
│  │Zookeeper │    │  Kafka   │    │  Consul  │         │
│  │  :2181   │◄───│  :9092    │    │  :8500   │         │
│  └──────────┘    └────┬──────┘    └──────────┘         │
│                      │                                │
│                      │ Port Mapping                   │
│                      │ 9092 → 9092 (internal)        │
│                      │ 29092 → 29092 (external)      │
└──────────────────────┼────────────────────────────────┘
                       │
                       │ localhost:29092
                       │
┌──────────────────────┼────────────────────────────────┐
│   Local Services     │                                │
│  ┌──────────────┐   │   ┌──────────────┐            │
│  │ iam-service  │───┼───│Patient-service│            │
│  │  :7020/7021  │   │   │    :7263     │            │
│  └──────────────┘   │   └──────────────┘            │
│                     │                                │
│  Connect to:        │                                │
│  localhost:29092    │                                │
└─────────────────────┘                                │
```

---

## ✅ Tại Sao Có Thể Kết Nối?

### 1. **Port Mapping trong docker-compose.yml**

```yaml
kafka:
  ports:
    - "9092:9092"     # Internal Docker network
    - "29092:29092"   # External (localhost) ← Services local dùng port này
  environment:
    KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:9092,PLAINTEXT_HOST://0.0.0.0:29092
    KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:9092,PLAINTEXT_HOST://localhost:29092
```

**Giải thích:**
- `29092:29092` → Map port 29092 từ container ra host
- Services local connect đến `localhost:29092`
- Docker forward connection vào container

### 2. **Configuration Đã Đúng**

**iam-service/appsettings.json:**
```json
{
  "Kafka": {
    "Host": "localhost:29092"  ✅ Đúng!
  }
}
```

**Patient-service/appsettings.json:**
```json
{
  "Kafka": {
    "Host": "localhost:29092"  ✅ Đúng!
  }
}
```

### 3. **KAFKA_ADVERTISED_LISTENERS**

```yaml
KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:9092,PLAINTEXT_HOST://localhost:29092
```

- `PLAINTEXT://kafka:9092` → Cho services trong Docker network
- `PLAINTEXT_HOST://localhost:29092` → Cho services local (host machine)

---

## 🚀 Cách Test

### Bước 1: Start Kafka trong Docker
```bash
docker-compose up -d zookeeper kafka
```

### Bước 2: Verify Kafka đang chạy
```bash
# Check container
docker ps | findstr kafka

# Check port
netstat -ano | findstr :29092

# Test connection
telnet localhost 29092
```

### Bước 3: Start Services Local
```bash
# Terminal 1: iam-service
cd iam-service
dotnet run

# Terminal 2: Patient-service
cd Patient-service
dotnet run
```

### Bước 4: Kiểm Tra Logs

**iam-service logs:**
```
✅ [Kafka] Topic exists: iam-service.user-created
✅ [IAMService] Kafka producer configured on localhost:29092
```

**Patient-service logs:**
```
🟢 Kafka consumer started. Listening on topic: iam-service.user-created
```

### Bước 5: Test Integration

**Create user:**
```bash
POST http://localhost:7020/api/auth/register
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "password123"
}
```

**Check Patient-service logs:**
```
✅ [Kafka] Received UserCreatedEvent for test@example.com
```

---

## ✅ Kết Luận

### **CÓ THỂ KẾT NỐI ĐƯỢC!** Vì:

1. ✅ **Port mapping đúng**: `29092:29092` expose ra ngoài
2. ✅ **Configuration đúng**: Services dùng `localhost:29092`
3. ✅ **KAFKA_ADVERTISED_LISTENERS đúng**: Có listener cho `localhost:29092`
4. ✅ **Network isolation không ảnh hưởng**: Port mapping bypass Docker network

---

## 🔍 So Sánh Các Scenarios

| Scenario | Kafka | Services | Kết Nối? |
|----------|-------|----------|----------|
| **1. Docker + Local** | Docker | Local | ✅ **CÓ** (dùng `localhost:29092`) |
| **2. Docker + Docker** | Docker | Docker | ✅ **CÓ** (dùng `kafka:9092`) |
| **3. Local + Local** | Local | Local | ✅ **CÓ** (dùng `localhost:9092`) |

**Scenario của bạn là #1 - HOÀN TOÀN ĐƯỢC!**

---

## 🎯 Lưu Ý Quan Trọng

### 1. **Port Phải Đúng**
- Services local → `localhost:29092` ✅
- Services trong Docker → `kafka:9092` ✅

### 2. **Kafka Phải Start Trước**
```bash
# Start Kafka trước
docker-compose up -d zookeeper kafka

# Đợi 10-15 giây cho Kafka fully start
docker logs kafka --tail 20

# Sau đó mới start services
```

### 3. **Firewall/Network**
- Đảm bảo port 29092 không bị block
- Windows Firewall có thể cần allow port

---

## 🚀 Quick Test

```bash
# 1. Start Kafka
docker-compose up -d zookeeper kafka

# 2. Wait và verify
docker logs kafka --tail 20

# 3. Test connection từ local
telnet localhost 29092
# Hoặc
Test-NetConnection localhost -Port 29092

# 4. Start services
# iam-service và Patient-service sẽ tự động kết nối
```

---

## ✅ Kết Luận

**Câu trả lời: CÓ, HOÀN TOÀN ĐƯỢC!**

- ✅ Kafka chạy trong Docker
- ✅ Services chạy local
- ✅ Kết nối qua `localhost:29092`
- ✅ Configuration đã đúng
- ✅ Port mapping đã setup sẵn

**Chỉ cần:**
1. Start Kafka: `docker-compose up -d kafka zookeeper`
2. Start services: `dotnet run`
3. Test integration!




















