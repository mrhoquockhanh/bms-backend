# Payment-service (minimal)

This is a small .NET 9 Web API microservice that manages Payments using an in-memory EF Core database. It provides endpoints to create and query payments.

How to run locally:

```powershell
cd Payment-service
dotnet run
```

Endpoints:
- POST /api/payments  -> create Payment
- GET /api/payments/{id} -> get by PaymentId
- GET /api/payments/by-patient/{patientId} -> get payments for a patient

VnPay integration:
- POST /api/vnpay/create-url -> create payment and return VnPay redirect URL (body: Payment JSON)
- GET /api/vnpay/return -> VnPay return URL (validate signature)

Fill `VnPay` section in `appsettings.json` with your TmnCode and HashSecret before using.

Using SQL Server (optional)
 - Update `ConnectionStrings:DefaultConnection` in `appsettings.json` with your SQL Server connection string.
 - Add EF Core tools if you want to run migrations locally: `dotnet tool install --global dotnet-ef` (if not installed).
 - Create an initial migration and update database:
 ```powershell
 cd Payment-service
 dotnet ef migrations add InitialCreate
 dotnet ef database update
 dotnet run
 ```

If `DefaultConnection` is not set, the app will fall back to an in-memory database.
