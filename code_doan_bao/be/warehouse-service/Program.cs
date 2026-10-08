using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using warehouse_service.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration and Kestrel settings from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var mongoClient = new MongoDB.Driver.MongoClient(connectionString);
builder.Services.AddDbContext<WarehouseDbContext>(options => options.UseMongoDB(mongoClient, "WarehouseDB"));
// Repositories
builder.Services.AddScoped<warehouse_service.Repositories.IEquipmentRepository, warehouse_service.Repositories.EquipmentRepository>();
builder.Services.AddScoped<warehouse_service.Repositories.IReagentRepository, warehouse_service.Repositories.ReagentRepository>();
// Services
builder.Services.AddScoped<warehouse_service.Services.Interface.IEquipmentService, warehouse_service.Services.Implement.EquipmentService>();
builder.Services.AddScoped<warehouse_service.Services.Interface.IReagentService, warehouse_service.Services.Implement.ReagentService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();