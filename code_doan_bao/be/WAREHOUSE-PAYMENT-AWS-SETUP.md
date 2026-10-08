# ✅ WAREHOUSE & PAYMENT SERVICES - AWS MESSAGING SETUP

## 📋 Tổng Quan

Đã thêm AWS SNS/SQS messaging infrastructure cho 2 services:
- ✅ **warehouse-service**
- ✅ **Payment-service**

**Lưu ý:** Cả 2 services này chưa có Kafka dependencies, nhưng đã được setup sẵn AWS messaging để:
- Đảm bảo tính nhất quán với các services khác
- Sẵn sàng sử dụng messaging khi cần trong tương lai

---

## 🔧 Chi Tiết Theo Service

### **1. warehouse-service**

#### **Messaging Infrastructure:**
- ✅ `Messaging/IMessagePublisher.cs` - Interface for publishing
- ✅ `Messaging/SnsMessagePublisher.cs` - SNS implementation
- ✅ `Messaging/IMessageConsumer.cs` - Interface for consuming
- ✅ `Messaging/SqsMessageConsumer.cs` - SQS implementation with DLQ

#### **Configuration:**
- ✅ `Program.cs` - AWS SDK registration
- ✅ `warehouse-service.csproj` - AWS SDK packages added
- ✅ `appsettings.json` - AWS configuration (created)

#### **AWS Configuration:**
```json
{
  "AWS": {
    "Region": "ap-southeast-1",
    "SNS": {
      "EquipmentCreatedTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:warehouse-service-equipment-created",
      "ReagentCreatedTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:warehouse-service-reagent-created"
    },
    "SQS": {
      "DeadLetterQueueUrl": "https://sqs.ap-southeast-1.amazonaws.com/123456789012/warehouse-service-dlq"
    }
  }
}
```

#### **Dependencies Added:**
- ✅ `AWSSDK.SimpleNotificationService` Version="3.7.400.50"
- ✅ `AWSSDK.SQS` Version="3.7.400.50"

---

### **2. Payment-service**

#### **Messaging Infrastructure:**
- ✅ `Messaging/IMessagePublisher.cs` - Interface for publishing
- ✅ `Messaging/SnsMessagePublisher.cs` - SNS implementation
- ✅ `Messaging/IMessageConsumer.cs` - Interface for consuming
- ✅ `Messaging/SqsMessageConsumer.cs` - SQS implementation with DLQ

#### **Configuration:**
- ✅ `Program.cs` - AWS SDK registration
- ✅ `Payment-service.csproj` - AWS SDK packages added
- ✅ `appsettings.json` - AWS configuration added

#### **AWS Configuration:**
```json
{
  "AWS": {
    "Region": "ap-southeast-1",
    "SNS": {
      "PaymentCreatedTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:payment-service-payment-created",
      "PaymentCompletedTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:payment-service-payment-completed"
    },
    "SQS": {
      "DeadLetterQueueUrl": "https://sqs.ap-southeast-1.amazonaws.com/123456789012/payment-service-dlq"
    }
  }
}
```

#### **Dependencies Added:**
- ✅ `AWSSDK.SimpleNotificationService` Version="3.7.400.50"
- ✅ `AWSSDK.SQS` Version="3.7.400.50"

---

## 📦 Dependencies

### **Added:**
- ✅ `AWSSDK.SimpleNotificationService` (both services)
- ✅ `AWSSDK.SQS` (both services)

### **Removed:**
- ❌ None (services didn't have Kafka)

---

## 🎯 Usage Examples

### **Publishing Events (Future Use):**

```csharp
// In warehouse-service
public class EquipmentService
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IConfiguration _configuration;

    public async Task CreateEquipmentAsync(EquipmentDto dto)
    {
        // ... create equipment logic ...

        // Publish event
        var topicArn = _configuration["AWS:SNS:EquipmentCreatedTopicArn"];
        await _messagePublisher.PublishAsync(topicArn, new EquipmentCreatedEvent
        {
            EquipmentId = equipment.Id,
            Name = equipment.Name,
            // ... other properties
        });
    }
}
```

### **Consuming Events (Future Use):**

```csharp
// Create a BackgroundService consumer
public class SomeEventSqsConsumer : BackgroundService
{
    private readonly IMessageConsumer _messageConsumer;
    private readonly IConfiguration _configuration;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = _configuration["AWS:SQS:SomeEventQueueUrl"];
        await _messageConsumer.StartConsumingAsync(
            queueUrl,
            async (messageBody, ct) =>
            {
                // Process message
                var event = JsonSerializer.Deserialize<SomeEvent>(messageBody);
                // ... handle event ...
                return true; // Success
            },
            stoppingToken);
    }
}
```

---

## ✅ Checklist

- [x] Create messaging abstractions (IMessagePublisher, IMessageConsumer)
- [x] Create SNS Publisher implementation
- [x] Create SQS Consumer implementation with DLQ
- [x] Update Program.cs to register AWS services
- [x] Update csproj files with AWS SDK
- [x] Create/Update appsettings.json with AWS config
- [x] No linter errors

---

## 🚀 Next Steps (When Needed)

1. **Define Event Models:**
   - Create event DTOs in `Events/Models/` folder
   - Example: `EquipmentCreatedEvent.cs`, `PaymentCompletedEvent.cs`

2. **Implement Event Publishing:**
   - Inject `IMessagePublisher` in services
   - Publish events after business operations

3. **Implement Event Consumers:**
   - Create BackgroundService consumers
   - Register in Program.cs as `AddHostedService`

4. **Create AWS Resources:**
   - SNS Topics (as configured in appsettings.json)
   - SQS Queues (subscribed to SNS topics)
   - DLQ Queues

5. **Update appsettings.json:**
   - Replace placeholder ARNs with actual AWS resource ARNs

---

## 🎉 Setup Complete!

Cả 2 services đã được setup sẵn AWS messaging infrastructure. Sẵn sàng sử dụng khi cần publish/consume events trong tương lai.

**Lưu ý:** Services này hiện tại chưa publish/consume events, nhưng infrastructure đã sẵn sàng. Chỉ cần:
1. Tạo event models
2. Inject `IMessagePublisher` và publish events
3. Tạo BackgroundService consumers nếu cần consume events



