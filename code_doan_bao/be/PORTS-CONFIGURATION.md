# 🔌 Cấu Hình Ports cho Patient-service và Testorder-service

## 📋 Tổng Quan

Tài liệu này giải thích chi tiết về cấu hình ports cho cả 2 service và cách chúng hoạt động trong môi trường development và Docker.

---

## 🏥 Patient-service

### 📝 Cấu hình Ports

#### 1. **appsettings.json** (Dùng cho Docker và môi trường production)
```json
{
  "ConsulConfig": {
    "ServicePort": 7263,    // REST API port (HTTPS trong Docker)
    "ServiceHost": "localhost"
  },
  "Grpc": {
    "Port": 7264            // gRPC port
  }
}
```

#### 2. **launchSettings.json** (Dùng cho Visual Studio/Rider local development)
```json
{
  "http": {
    "applicationUrl": "http://localhost:5109"    // HTTP endpoint (local)
  },
  "https": {
    "applicationUrl": "https://localhost:7263;http://localhost:5109"  // HTTPS + HTTP (local)
  }
}
```

### 🔄 Cách Hoạt Động

#### **Local Development (Visual Studio/Rider)**
- Kestrel **KHÔNG** cấu hình cứng ports → Sử dụng ports từ `launchSettings.json`
- Profile **"http"**: `http://localhost:5109` → Swagger UI tự động mở tại `http://localhost:5109/`
- Profile **"https"**: `https://localhost:7263` → Swagger UI tự động mở tại `https://localhost:7263/`

#### **Docker**
- Kestrel **cấu hình cứng** ports từ `appsettings.json`
- REST API: Port `7263` (HTTP)
- gRPC: Port `7264` (HTTP/2)

### 📍 Truy Cập Swagger UI

| Môi trường | URL Swagger |
|------------|-------------|
| Local HTTP | `http://localhost:5109/` |
| Local HTTPS | `https://localhost:7263/` |
| Docker | `http://localhost:7263/` (nếu port mapped) |

---

## 🧪 Testorder-service

### 📝 Cấu hình Ports

#### 1. **appsettings.json** (Dùng cho Docker và môi trường production)
```json
{
  "ConsulConfig": {
    "ServicePort": 7105,    // REST API port (HTTPS trong Docker)
    "ServiceHost": "localhost"
  },
  "Grpc": {
    "Port": 7106            // gRPC port
  }
}
```

#### 2. **launchSettings.json** (Dùng cho Visual Studio/Rider local development)
```json
{
  "http": {
    "applicationUrl": "http://localhost:5275"    // HTTP endpoint (local)
  },
  "https": {
    "applicationUrl": "https://localhost:7105;http://localhost:5275"  // HTTPS + HTTP (local)
  }
}
```

### 🔄 Cách Hoạt Động

#### **Local Development (Visual Studio/Rider)**
- Kestrel **KHÔNG** cấu hình cứng ports → Sử dụng ports từ `launchSettings.json`
- Profile **"http"**: `http://localhost:5275` → Swagger UI tự động mở tại `http://localhost:5275/`
- Profile **"https"**: `https://localhost:7105` → Swagger UI tự động mở tại `https://localhost:7105/`

#### **Docker**
- Kestrel **cấu hình cứng** ports từ `appsettings.json`
- REST API: Port `7105` (HTTP)
- gRPC: Port `7106` (HTTP/2)

### 📍 Truy Cập Swagger UI

| Môi trường | URL Swagger |
|------------|-------------|
| Local HTTP | `http://localhost:5275/` |
| Local HTTPS | `https://localhost:7105/` |
| Docker | `http://localhost:7105/` (nếu port mapped) |

---

## 🔍 So Sánh Ports

### Patient-service
| Loại | Local (launchSettings) | Docker (appsettings) | Ghi chú |
|------|------------------------|----------------------|---------|
| HTTP REST | `5109` | `7263` | Khác nhau giữa local và Docker |
| HTTPS REST | `7263` | `7263` | **Giống nhau** |
| gRPC | `7264` | `7264` | Giống nhau |

### Testorder-service
| Loại | Local (launchSettings) | Docker (appsettings) | Ghi chú |
|------|------------------------|----------------------|---------|
| HTTP REST | `5275` | `7105` | Khác nhau giữa local và Docker |
| HTTPS REST | `7105` | `7105` | **Giống nhau** |
| gRPC | `7106` | `7106` | Giống nhau |

---

## ⚙️ Cấu Hình Swagger

### Program.cs Configuration

Cả 2 service đều có cấu hình tương tự:

```csharp
// Swagger chỉ bật trong Development mode hoặc khi chạy local
if (app.Environment.IsDevelopment() || !isRunningInContainer)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.RoutePrefix = string.Empty; // Swagger UI ở root "/"
        options.DisplayRequestDuration();
    });
}
```

### launchSettings.json Configuration

```json
{
  "launchBrowser": true,    // ✅ Tự động mở browser
  "launchUrl": "/"          // ✅ Mở tại Swagger UI (root)
}
```

---

## 🚀 Hướng Dẫn Sử Dụng

### Chạy Local Development

1. **Chọn Profile trong Visual Studio/Rider:**
   - **"http"**: Chạy HTTP, mở `http://localhost:5109/` (Patient) hoặc `http://localhost:5275/` (Testorder)
   - **"https"**: Chạy HTTPS, mở `https://localhost:7263/` (Patient) hoặc `https://localhost:7105/` (Testorder)

2. **Swagger UI sẽ tự động mở** vì:
   - `launchBrowser: true`
   - `launchUrl: "/"`
   - Swagger RoutePrefix = `""` (root path)

### Chạy Docker

```bash
docker-compose up
```

- Service sẽ chạy với ports từ `appsettings.json`
- Swagger chỉ bật nếu `ASPNETCORE_ENVIRONMENT=Development`

---

## ⚠️ Lưu Ý Quan Trọng

1. **Không thay đổi ports mặc định** trong `launchSettings.json` trừ khi có lý do cụ thể
2. **Ports trong `appsettings.json`** dùng cho Docker/production
3. **Ports trong `launchSettings.json`** dùng cho local development
4. **Kestrel chỉ cấu hình cứng khi chạy trong Docker** → Cho phép `launchSettings.json` kiểm soát ports khi local
5. **Swagger RoutePrefix = ""** → Swagger UI ở root "/", không phải "/swagger"

---

## 🔧 Troubleshooting

### Swagger không tự động mở
- ✅ Kiểm tra `launchBrowser: true` trong `launchSettings.json`
- ✅ Kiểm tra `launchUrl: "/"` (không phải `""`)
- ✅ Kiểm tra `ASPNETCORE_ENVIRONMENT=Development`
- ✅ Kiểm tra Swagger middleware được bật trong `Program.cs`

### Port bị conflict
- ✅ Đảm bảo không có service khác đang dùng port
- ✅ Kiểm tra ports trong `launchSettings.json` không conflict với `appsettings.json`

### Swagger không hiển thị
- ✅ Truy cập trực tiếp URL: `http://localhost:5109/` (Patient) hoặc `http://localhost:5275/` (Testorder)
- ✅ Kiểm tra console logs xem service có start đúng không
- ✅ Kiểm tra middleware order trong `Program.cs` (Swagger phải trước UseRouting)

---

**Tạo ngày:** $(date)
**Cập nhật lần cuối:** $(date)









