# 📋 BÁO CÁO RÀ SOÁT CẤU HÌNH KAFKA & gRPC

## ✅ **TỔNG KẾT**

Đã rà soát và sửa các vấn đề trong cấu hình Kafka, gRPC và Docker Compose cho 3 services:
- ✅ **iam-service**
- ✅ **Patient-service**
- ✅ **Testorder-service**

---

## 🔍 **CÁC VẤN ĐỀ ĐÃ PHÁT HIỆN VÀ SỬA**

### **1. iam-service** ✅

#### **Vấn đề:**
- ❌ Thiếu gRPC port mapping (7021:7021) trong docker-compose
- ❌ Thiếu ConsulConfig environment variables
- ❌ Thiếu DOTNET_RUNNING_IN_CONTAINER flag
- ❌ Thiếu depends_on postgres

#### **Đã sửa:**
- ✅ Thêm port mapping `7021:7021` cho gRPC
- ✅ Thêm đầy đủ ConsulConfig environment variables:
  - `ConsulConfig__Address: "http://consul:8500"`
  - `ConsulConfig__ServiceName: "iam-service"`
  - `ConsulConfig__ServiceId: "iam-service-1"`
  - `ConsulConfig__ServiceHost: "iam-service"`
  - `ConsulConfig__ServicePort: "7020"`
  - `ConsulConfig__HealthCheckEndpoint: "/health"`
- ✅ Thêm `DOTNET_RUNNING_IN_CONTAINER: "true"`
- ✅ Thêm `Grpc__Port: "7021"`
- ✅ Thêm `depends_on: postgres`

---

### **2. Patient-service** ✅

#### **Vấn đề:**
- ❌ **CRITICAL**: Connection string sai - đang dùng PostgreSQL nhưng service cần SQL Server!
- ❌ Thiếu SQL Server service trong docker-compose
- ❌ Thiếu Kafka GroupId environment variable
- ❌ Thiếu Consul environment variables
- ❌ Thiếu gRPC settings environment variables

#### **Đã sửa:**
- ✅ **Thêm SQL Server service** vào docker-compose:
  ```yaml
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "12345"
      MSSQL_PID: "Express"
    ports:
      - "1433:1433"
  ```
- ✅ **Sửa connection string** từ PostgreSQL sang SQL Server:
  - Trước: `Host=postgres;Port=5432;Database=PatientService;Username=postgres;Password=12345`
  - Sau: `Server=sqlserver;Database=PatientService;User Id=sa;Password=12345;TrustServerCertificate=True;`
- ✅ Thêm `Kafka__GroupId: "patient-service-consumer"`
- ✅ Thêm `Consul__Address: "http://consul:8500"`
- ✅ Thêm `GrpcSettings__IamServiceUrl: "http://iam-service:7021"`
- ✅ Thêm `depends_on: sqlserver`
- ✅ Thêm `DOTNET_RUNNING_IN_CONTAINER: "true"`

---

### **3. Testorder-service** ✅

#### **Vấn đề:**
- ❌ **CRITICAL**: Thiếu TẤT CẢ environment variables (Kafka, Consul, Database)
- ❌ Thiếu SQL Server service dependency
- ❌ Thiếu Consul dependency
- ❌ Thiếu ASPNETCORE_URLS

#### **Đã sửa:**
- ✅ Thêm đầy đủ Kafka environment variables:
  - `Kafka__Host: "kafka:9092"`
  - `Kafka__GroupId: "testorder-service-consumer"`
  - `Kafka__Topics__AppointmentCreated: "testorder-service.appointment-created"`
  - `Kafka__Topics__TestResultCreated: "testorder-service.testresult-created"`
  - `Kafka__Topics__UserCreated: "iam-service.user-created"`
- ✅ Thêm Database connection string:
  - `ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=TestOrderManagementDB;User Id=sa;Password=12345;TrustServerCertificate=True;"`
- ✅ Thêm Consul environment variable:
  - `Consul__Address: "http://consul:8500"`
- ✅ Thêm gRPC settings:
  - `GrpcSettings__IamServiceUrl: "http://iam-service:7021"`
- ✅ Thêm `ASPNETCORE_URLS: "http://+:7300"`
- ✅ Thêm `depends_on: sqlserver` và `consul`
- ✅ Thêm `DOTNET_RUNNING_IN_CONTAINER: "true"`

---

## 📊 **BẢNG SO SÁNH TRƯỚC/SAU**

| Service | Component | Trước | Sau |
|---------|-----------|-------|-----|
| **iam-service** | gRPC Port | ❌ Không có | ✅ 7021:7021 |
| **iam-service** | Consul Config | ❌ Thiếu | ✅ Đầy đủ |
| **Patient-service** | Database | ❌ PostgreSQL (SAI) | ✅ SQL Server |
| **Patient-service** | SQL Server Service | ❌ Không có | ✅ Đã thêm |
| **Patient-service** | Kafka GroupId | ❌ Thiếu | ✅ Đã thêm |
| **Testorder-service** | Kafka Config | ❌ Thiếu hoàn toàn | ✅ Đầy đủ |
| **Testorder-service** | Database Config | ❌ Thiếu | ✅ Đã thêm |
| **Testorder-service** | Consul Config | ❌ Thiếu | ✅ Đã thêm |

---

## ✅ **CẤU HÌNH KAFKA - KIỂM TRA**

### **iam-service**
- ✅ **Producer**: `KafkaProducerService` đã được register
- ✅ **Topic**: `iam-service.user-created`
- ✅ **Host**: `kafka:9092` (Docker) / `localhost:29092` (Local)
- ✅ **Auto-detection**: Environment-aware

### **Patient-service**
- ✅ **Consumer**: `KafkaConsumerService` (BackgroundService)
- ✅ **Topic**: `iam-service.user-created` (consume)
- ✅ **GroupId**: `patient-service-consumer`
- ✅ **Host**: `kafka:9092` (Docker) / `localhost:29092` (Local)

### **Testorder-service**
- ✅ **Producer**: `KafkaProducerService` đã được register
- ✅ **Consumer**: `KafkaConsumerService` (BackgroundService)
- ✅ **Topics**:
  - Publish: `testorder-service.appointment-created`
  - Publish: `testorder-service.testresult-created`
  - Consume: `iam-service.user-created`
- ✅ **GroupId**: `testorder-service-consumer`
- ✅ **Host**: `kafka:9092` (Docker) / `localhost:29092` (Local)

---

## ✅ **CẤU HÌNH gRPC - KIỂM TRA**

### **iam-service (gRPC Server)**
- ✅ **Port**: 7021
- ✅ **Protocol**: HTTP/2
- ✅ **Service**: `UserGrpcService`
- ✅ **Registration**: `app.MapGrpcService<UserGrpcService>()`
- ✅ **Port Mapping**: `7021:7021` (đã thêm)

### **Patient-service (gRPC Client)**
- ✅ **Client**: `IamGrpcClient` wrapper
- ✅ **Discovery**: Consul với fallback
- ✅ **Port**: 7021 (fixed)
- ✅ **Address**: `iam-service:7021` (Docker) / `localhost:7021` (Local)

### **Testorder-service (gRPC Client)**
- ✅ **Client**: `IamGrpcClient` wrapper
- ✅ **Discovery**: Consul với fallback
- ✅ **Port**: 7021 (fixed)
- ✅ **Address**: `iam-service:7021` (Docker) / `localhost:7021` (Local)
- ✅ **Methods**: `GetUserByIdAsync`, `GetUserByEmailAsync`, `ValidateUserTokenAsync`

---

## 🐳 **DOCKER COMPOSE - KIỂM TRA**

### **Services Dependencies**
```
iam-service
  ├── kafka (healthy)
  ├── consul (started)
  └── postgres (started) ✅ MỚI THÊM

patient-service
  ├── kafka (healthy)
  ├── consul (started)
  └── sqlserver (started) ✅ MỚI THÊM

testorder-service
  ├── kafka (healthy)
  ├── consul (started) ✅ MỚI THÊM
  └── sqlserver (started) ✅ MỚI THÊM
```

### **Port Mappings**
- ✅ **iam-service**: `7020:7020` (REST), `7021:7021` (gRPC) ✅ MỚI THÊM
- ✅ **patient-service**: `7263:7263` (REST)
- ✅ **testorder-service**: `7300:7300` (REST)
- ✅ **sqlserver**: `1433:1433` ✅ MỚI THÊM

### **Volumes**
- ✅ `sqlserver_data` ✅ MỚI THÊM

---

## 🚀 **HƯỚNG DẪN CHẠY**

### **1. Start Dependencies**
```bash
docker-compose up -d zookeeper kafka consul postgres sqlserver
```

### **2. Start Services**
```bash
docker-compose up -d iam-service patient-service testorder-service
```

### **3. Verify**
```bash
# Check services
docker-compose ps

# Check Kafka topics
docker exec kafka kafka-topics --bootstrap-server localhost:9092 --list

# Check Consul services
curl http://localhost:8500/v1/agent/services
```

---

## ✅ **KẾT LUẬN**

### **Tất cả các vấn đề đã được sửa:**
1. ✅ iam-service: Thêm gRPC port và Consul config
2. ✅ Patient-service: Sửa database từ PostgreSQL sang SQL Server
3. ✅ Testorder-service: Thêm đầy đủ environment variables
4. ✅ Thêm SQL Server service vào docker-compose
5. ✅ Tất cả services đã có đầy đủ dependencies và environment variables

### **Hệ thống sẵn sàng để chạy!** 🎉

---

## 📝 **LƯU Ý**

1. **SQL Server**: Cần thời gian khởi động (~30-60 giây). Đảm bảo SQL Server đã sẵn sàng trước khi start services.

2. **Kafka**: Services sẽ tự động retry kết nối nếu Kafka chưa sẵn sàng.

3. **Consul**: Services sẽ fallback về config nếu Consul không available.

4. **Local Development**: Khi chạy local (không trong Docker), các services sẽ tự động detect và dùng `localhost` thay vì Docker service names.

