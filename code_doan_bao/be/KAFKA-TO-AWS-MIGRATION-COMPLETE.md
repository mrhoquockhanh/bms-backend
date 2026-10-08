# ✅ KAFKA TO AWS SNS/SQS MIGRATION - COMPLETE

## 📋 Tổng Quan

Đã hoàn thành việc refactor toàn bộ hệ thống từ **Apache Kafka** sang **AWS SNS + SQS** cho 3 microservices:
- ✅ **iam-service**
- ✅ **Patient-service**  
- ✅ **Testorder-service**

---

## 🔄 Thay Đổi Chính

### **1. Messaging Infrastructure**

#### **Trước (Kafka):**
- Kafka Producers → Publish events to Kafka topics
- Kafka Consumers → Consume events from Kafka topics
- Kafka Topics → Event routing mechanism

#### **Sau (AWS SNS/SQS):**
- **SNS Publishers** → Publish events to SNS topics (broadcast)
- **SQS Consumers** → Consume events from SQS queues (per-service)
- **SNS Topics** → Event broadcast mechanism
- **SQS Queues** → Per-service message queues (subscribed to SNS topics)

---

## 📁 Cấu Trúc Mới

### **Abstraction Layer**
Mỗi service có abstraction layer riêng:

```
{service}/Messaging/
├── IMessagePublisher.cs      # Interface for publishing
├── SnsMessagePublisher.cs    # SNS implementation
├── IMessageConsumer.cs       # Interface for consuming
└── SqsMessageConsumer.cs      # SQS implementation with DLQ support
```

### **Event Consumers**
```
{service}/Events/Consumers/
├── {EventName}SqsConsumer.cs  # BackgroundService consuming from SQS
```

---

## 🔧 Chi Tiết Theo Service

### **1. iam-service**

#### **Publishes:**
- ✅ `UserCreatedEvent` → SNS Topic: `AWS:SNS:UserCreatedTopicArn`

#### **Consumes:**
- ✅ `EmailNotificationEvent` → SQS Queue: `AWS:SQS:EmailNotificationQueueUrl`

#### **Files Changed:**
- ✅ `Service/Implement/AccountService.cs` - Uses `IMessagePublisher` instead of `IKafkaProducerService`
- ✅ `Repositories/Implement/AuthRepo.cs` - Uses `IMessagePublisher` instead of `IKafkaProducerService`
- ✅ `Events/Consumers/EmailNotificationSqsConsumer.cs` - New SQS consumer
- ✅ `Program.cs` - AWS SDK registration, removed Kafka config
- ✅ `appsettings.json` - AWS configuration added, Kafka removed
- ✅ `iam-service.csproj` - AWS SDK packages, Kafka packages removed

#### **Files Deleted:**
- ❌ `Events/Producers/KafkaProducerService.cs`
- ❌ `Events/Producers/IKafkaProducerService.cs`
- ❌ `Events/Consumers/EmailNotificationConsumer.cs` (old Kafka version)
- ❌ `Configurations/KafkaConfigExtensions.cs`

---

### **2. Patient-service**

#### **Publishes:**
- ❌ None

#### **Consumes:**
- ✅ `UserCreatedEvent` → SQS Queue: `AWS:SQS:UserCreatedQueueUrl`

#### **Files Changed:**
- ✅ `Events/Consumers/UserCreatedSqsConsumer.cs` - New SQS consumer
- ✅ `Program.cs` - AWS SQS registration, removed Kafka consumer
- ✅ `appsettings.json` - AWS configuration added, Kafka removed
- ✅ `Patient-service.csproj` - AWS SDK packages, Kafka packages removed

#### **Files Deleted:**
- ❌ `Events/Consumers/KafkaConsumerService.cs`
- ❌ `Events/Consumers/IKafkaConsumerService.cs`
- ❌ `Configurations/KafkaConfigExtensions.cs`

---

### **3. Testorder-service**

#### **Publishes:**
- ✅ `EmailNotificationEvent` → SNS Topic: `AWS:SNS:EmailNotificationTopicArn`
- ⚠️ `AppointmentCreatedEvent` → SNS Topic: `AWS:SNS:AppointmentCreatedTopicArn` (configured, not used yet)
- ⚠️ `TestResultCreatedEvent` → SNS Topic: `AWS:SNS:TestResultCreatedTopicArn` (configured, not used yet)

#### **Consumes:**
- ✅ `UserCreatedEvent` → SQS Queue: `AWS:SQS:UserCreatedQueueUrl`

#### **Files Changed:**
- ✅ `Services/Implement/EmailNotificationService.cs` - Uses `IMessagePublisher` instead of `IKafkaProducerService`
- ✅ `Events/Consumers/UserCreatedSqsConsumer.cs` - New SQS consumer
- ✅ `Program.cs` - AWS SDK registration, removed Kafka config
- ✅ `appsettings.json` - AWS configuration added, Kafka removed
- ✅ `Testorder-service.csproj` - AWS SDK packages, Kafka packages removed

#### **Files Deleted:**
- ❌ `Events/Producers/KafkaProducerService.cs`
- ❌ `Events/Producers/IKafkaProducerService.cs`
- ❌ `Events/Consumers/KafkaConsumerService.cs`
- ❌ `Events/Consumers/IKafkaConsumerService.cs`
- ❌ `Configurations/KafkaConfigExtensions.cs`

---

## 📦 Dependencies

### **Removed:**
- ❌ `Confluent.Kafka` (all services)
- ❌ `MassTransit` (iam-service)
- ❌ `MassTransit.Kafka` (iam-service, Patient-service)

### **Added:**
- ✅ `AWSSDK.SimpleNotificationService` (iam-service, Testorder-service)
- ✅ `AWSSDK.SQS` (all services)

---

## ⚙️ Configuration

### **appsettings.json Structure:**

```json
{
  "AWS": {
    "Region": "ap-southeast-1",
    "SNS": {
      "UserCreatedTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:iam-service-user-created",
      "EmailNotificationTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:iam-service-email-notification",
      "AppointmentCreatedTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:testorder-service-appointment-created",
      "TestResultCreatedTopicArn": "arn:aws:sns:ap-southeast-1:123456789012:testorder-service-testresult-created"
    },
    "SQS": {
      "UserCreatedQueueUrl": "https://sqs.ap-southeast-1.amazonaws.com/123456789012/{service}-user-created-queue",
      "EmailNotificationQueueUrl": "https://sqs.ap-southeast-1.amazonaws.com/123456789012/iam-service-email-notification-queue",
      "DeadLetterQueueUrl": "https://sqs.ap-southeast-1.amazonaws.com/123456789012/{service}-dlq"
    }
  }
}
```

---

## 🔐 AWS Credentials

**Assumption:** AWS credentials are injected via **IAM Roles for ECS** (as specified in requirements).

The code uses default credential chain:
```csharp
new AmazonSimpleNotificationServiceClient(regionEndpoint);
new AmazonSQSClient(regionEndpoint);
```

This will automatically use:
1. IAM Role credentials (when running on ECS)
2. Environment variables (AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY)
3. AWS credentials file (~/.aws/credentials)
4. EC2 instance profile (when running on EC2)

---

## 🎯 Features Implemented

### **1. SNS Publisher**
- ✅ Generic `PublishAsync<T>()` method
- ✅ JSON serialization
- ✅ Message type metadata
- ✅ Error handling and logging

### **2. SQS Consumer**
- ✅ Long polling (20 seconds)
- ✅ Batch processing (up to 10 messages)
- ✅ **Dead Letter Queue (DLQ) support**
- ✅ Automatic retry (max 3 attempts)
- ✅ Error handling and logging
- ✅ BackgroundService implementation

### **3. Event Handling**
- ✅ All existing events preserved
- ✅ Same event DTOs (no changes)
- ✅ Same business logic (no changes)

---

## 📊 Event Flow

### **UserCreatedEvent Flow:**
```
iam-service (AccountService/AuthRepo)
  → IMessagePublisher.PublishAsync()
  → SNS Topic: iam-service-user-created
  → SQS Queues (subscribed):
    ├── patient-service-user-created-queue
    └── testorder-service-user-created-queue
  → BackgroundService consumers process messages
```

### **EmailNotificationEvent Flow:**
```
testorder-service (EmailNotificationService)
  → IMessagePublisher.PublishAsync()
  → SNS Topic: iam-service-email-notification
  → SQS Queue (subscribed):
    └── iam-service-email-notification-queue
  → EmailNotificationSqsConsumer processes message
  → EmailService sends email
```

---

## ✅ Checklist

- [x] Remove all Kafka dependencies
- [x] Add AWS SDK dependencies
- [x] Create messaging abstractions
- [x] Implement SNS Publisher
- [x] Implement SQS Consumer with DLQ
- [x] Update all services to use AWS
- [x] Update appsettings.json
- [x] Update csproj files
- [x] Delete all Kafka files
- [x] Remove Kafka configuration
- [x] Preserve all business logic
- [x] Preserve all event DTOs

---

## 🚀 Deployment Notes

### **AWS Resources Required:**

1. **SNS Topics:**
   - `iam-service-user-created`
   - `iam-service-email-notification`
   - `testorder-service-appointment-created` (optional, for future use)
   - `testorder-service-testresult-created` (optional, for future use)

2. **SQS Queues:**
   - `patient-service-user-created-queue` (subscribed to `iam-service-user-created`)
   - `testorder-service-user-created-queue` (subscribed to `iam-service-user-created`)
   - `iam-service-email-notification-queue` (subscribed to `iam-service-email-notification`)
   - `patient-service-dlq`
   - `testorder-service-dlq`
   - `iam-service-dlq`

3. **IAM Permissions (for ECS Task Role):**
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "sns:Publish",
        "sqs:SendMessage",
        "sqs:ReceiveMessage",
        "sqs:DeleteMessage",
        "sqs:GetQueueAttributes"
      ],
      "Resource": "*"
    }
  ]
}
```

---

## 📝 Next Steps

1. **Create AWS Resources:**
   - Create SNS topics
   - Create SQS queues
   - Subscribe SQS queues to SNS topics
   - Configure DLQ redrive policies

2. **Update appsettings.json:**
   - Replace placeholder ARNs with actual AWS resource ARNs
   - Replace placeholder queue URLs with actual queue URLs

3. **Test:**
   - Test event publishing
   - Test event consumption
   - Test DLQ functionality
   - Test error handling

4. **Monitor:**
   - CloudWatch metrics for SNS/SQS
   - DLQ message count
   - Consumer lag (if applicable)

---

## 🎉 Migration Complete!

Tất cả code đã được refactor thành công. Hệ thống sẵn sàng deploy lên AWS ECS với SNS/SQS messaging.

**Lưu ý:** Cần tạo AWS resources (SNS topics, SQS queues) và cập nhật ARNs/URLs trong appsettings.json trước khi deploy.



