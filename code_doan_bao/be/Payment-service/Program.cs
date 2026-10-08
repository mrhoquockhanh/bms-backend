using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using payment_service.Data;
using payment_service.Repositories;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Prefer SQL Server if configured, otherwise use InMemory for quick dev
var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(defaultConn))
{
    var mongoClient = new MongoDB.Driver.MongoClient(defaultConn);
    builder.Services.AddDbContext<PaymentDbContext>(options =>
        options.UseMongoDB(mongoClient, "PaymentDB"));
}
else
{
    builder.Services.AddDbContext<PaymentDbContext>(options =>
        options.UseInMemoryDatabase("PaymentsDb"));
}

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// VnPay api
builder.Services.AddSingleton<IVnpay, Vnpay>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
