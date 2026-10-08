# 🚀 Cách Start Kafka

## ⚠️ **Bạn Đang Ở Thư Mục Sai!**

Script `start-kafka.bat` và `start-kafka.ps1` nằm ở **root directory**, không phải trong `Testorder-service`.

---

## ✅ **CÁCH 1: Navigate Về Root Directory (Khuyên Dùng)**

### **PowerShell:**
```powershell
# Navigate về root
cd ..

# Hoặc về root trực tiếp
cd D:\OJT_PROJECT\lmsystem-main\LMSystem\lmsystem\LMSystem

# Sau đó chạy script
.\start-kafka.bat
```

### **Command Prompt:**
```cmd
cd ..
start-kafka.bat
```

---

## ✅ **CÁCH 2: Chạy Trực Tiếp Docker Compose (Đơn Giản Nhất)**

**Từ bất kỳ thư mục nào:**

```powershell
# Navigate về root (nơi có docker-compose.yml)
cd D:\OJT_PROJECT\lmsystem-main\LMSystem\lmsystem\LMSystem

# Start Kafka
docker-compose up -d zookeeper kafka
```

**Hoặc từ thư mục hiện tại (Testorder-service):**

```powershell
# Navigate về root
cd ..

# Start Kafka
docker-compose up -d zookeeper kafka
```

---

## ✅ **CÁCH 3: Chạy Script Với Đường Dẫn Đầy Đủ**

```powershell
# Từ thư mục Testorder-service
& "..\start-kafka.bat"

# Hoặc
& "D:\OJT_PROJECT\lmsystem-main\LMSystem\lmsystem\LMSystem\start-kafka.bat"
```

---

## 🎯 **QUICK COMMAND (Từ Testorder-service)**

```powershell
# Navigate về root và start Kafka
cd .. ; docker-compose up -d zookeeper kafka
```

---

## ✅ **VERIFY SAU KHI START**

```powershell
# Check containers
docker ps | Select-String "kafka"

# Test connection
Test-NetConnection localhost -Port 29092
```

**Expected:**
```
TcpTestSucceeded : True
```

---

## 📝 **TÓM TẮT**

**Từ thư mục `Testorder-service`:**

```powershell
# Option 1: Navigate về root rồi chạy script
cd ..
.\start-kafka.bat

# Option 2: Chạy docker-compose trực tiếp
cd ..
docker-compose up -d zookeeper kafka

# Option 3: Một dòng
cd .. ; docker-compose up -d zookeeper kafka
```

**Sau đó đợi 10-15 giây và restart service!** ✅



