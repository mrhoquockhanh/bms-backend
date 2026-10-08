using Consul;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Grpc.Net.Client;
using GrpcAuth_Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Patient_service;
using Patient_service.GrpcServices;
using Patient_service.MappingProfiles;
using Patient_service.Repositories;
using Patient_service.Repositories.Interface;
using Repositories.Interface;
using Service.Implement;
using Service.Interface;
using Service.Interface_service.Repositories.Interface;
using System.Text.Json.Serialization;

// Đã loại bỏ using Microsoft.AspNetCore.Authentication.JwtBearer, Microsoft.IdentityModel.Tokens, System.Text

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

// ---------------------- Configuration ----------------------
var configuration = builder.Configuration;
var env = builder.Environment;

// Read ports from configuration - Environment variables take precedence
var restPort = Environment.GetEnvironmentVariable("ConsulConfig__ServicePort") != null
    ? int.Parse(Environment.GetEnvironmentVariable("ConsulConfig__ServicePort")!)
    : configuration.GetValue<int>("ConsulConfig:ServicePort", 7263);
    
var grpcPort = Environment.GetEnvironmentVariable("Grpc__Port") != null
    ? int.Parse(Environment.GetEnvironmentVariable("Grpc__Port")!)
    : configuration.GetValue<int>("Grpc:Port", 7264);

var consulAddress = Environment.GetEnvironmentVariable("ConsulConfig__Address")
                    ?? configuration["ConsulConfig:Address"]
                    ?? configuration["Consul:Address"]
                    ?? "http://localhost:8500";

// Detect if running in container
var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Configure Kestrel based on environment
// ConfigureKestrel based on environment (same pattern as iam-service)
builder.WebHost.ConfigureKestrel(options =>
{
    if (isRunningInContainer)
    {
        // Docker: HTTP only
        // HTTP/REST API - Use HTTP/1.1
        options.ListenAnyIP(restPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        });
        
        // gRPC - HTTP/2 for gRPC communication
        options.ListenAnyIP(grpcPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    }
    else
    {
        // Local: HTTPS for REST API, HTTP/2 for gRPC
        // HTTPS/REST API - Use HTTP/1.1 with TLS
        options.ListenAnyIP(restPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            listenOptions.UseHttps(); // Use development certificate
        });
        
        // gRPC - HTTP/2 for gRPC communication
        options.ListenAnyIP(grpcPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    }
});

// ---------------------- 1. EF Core (DbContext) ----------------------
// Auto-detect database server based on environment
var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var mongoClient = new MongoDB.Driver.MongoClient(connectionString);
builder.Services.AddDbContext<PatientServiceContext>(options =>
{
    options.UseMongoDB(mongoClient, "PatientService");
});

// ---------------------- 2. AutoMapper ----------------------
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(PatientMappingProfile).Assembly);
});

// ---------------------- 3. Register Repositories ----------------------
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<ILabTestRepository, LabTestRepository>();
builder.Services.AddScoped<ILabCriterionRepository, LabCriterionRepository>();

// ---------------------- 4. Register Services (business layer) ----------------------
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<ILabTestService, LabTestService>();
builder.Services.AddScoped<ILabCriterionService, LabCriterionService>();


builder.Services.AddHttpClient();

// ---------------------- 4. gRPC Server Configuration ----------------------
builder.Services.AddGrpc();

// ---------------------- 5. Consul Client ----------------------
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(cfg =>
{
    cfg.Address = new Uri(consulAddress);
}));

// ---------------------- 6. gRPC Client dynamic discovery from Consul ----------------------
// Register IamGrpcClient wrapper (recommended - handles Consul discovery and fallback)
builder.Services.AddSingleton<Patient_service.GrpcClients.IamGrpcClient>();

// Register direct UserServiceClient for backward compatibility
// CRITICAL: Use service name in Docker, localhost in local
builder.Services.AddSingleton(sp =>
{
    var consul = sp.GetRequiredService<IConsulClient>();
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<UserService.UserServiceClient>>();

    // Fix: Default to service name in Docker, localhost in local
    string address = isRunningInContainer ? "iam-service" : "localhost";
    int grpcPort = 7021;
    bool useHttps = false;

    try
    {
        var services = consul.Agent.Services().GetAwaiter().GetResult();
        var iamService = services.Response.Values.FirstOrDefault(s => s.Service == "iam-service");

        if (iamService != null)
        {
            // Use address from Consul, but fallback to service name if address is empty or 0.0.0.0
            address = iamService.Address;
            if (string.IsNullOrEmpty(address) || address == "0.0.0.0")
            {
                address = isRunningInContainer ? "iam-service" : "localhost";
            }
            logger.LogInformation("🔗 Discovered IAMService in Consul: {Address}", address);
        }
        else
        {
            // Fallback to configuration or service name/localhost
            var fallbackUrl = config["GrpcSettings:IamServiceUrl"];
            if (!string.IsNullOrEmpty(fallbackUrl))
            {
                var urlParts = fallbackUrl.Replace("http://", "").Replace("https://", "").Split(':');
                address = urlParts[0];
                useHttps = fallbackUrl.StartsWith("https://");
                logger.LogWarning("⚠️ IAMService not found in Consul, using config fallback: {Address}", address);
            }
            else
            {
                // Final fallback: service name in Docker, localhost in local
                address = isRunningInContainer ? "iam-service" : "localhost";
                logger.LogWarning("⚠️ IAMService not found in Consul, using default fallback: {Address}", address);
            }
        }
    }
    catch (Exception ex)
    {
        // If Consul fails, use fallback based on environment
        address = isRunningInContainer ? "iam-service" : "localhost";
        logger.LogWarning(ex, "⚠️ Consul discovery failed, using fallback: {Address}", address);
    }

    var protocol = useHttps ? "https" : "http";
    var grpcAddress = $"{protocol}://{address}:{grpcPort}";
    logger.LogInformation("🔗 Creating gRPC channel to: {Address}", grpcAddress);

    var channel = GrpcChannel.ForAddress(grpcAddress, new GrpcChannelOptions
    {
        HttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }
    });

    return new UserService.UserServiceClient(channel);
});

// ---------------------- 7. Controllers + JSON options ----------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Encoder =
    System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);


    });

// ---------------------- 8. Swagger ----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Patient Service API",
        Version = "v1"
    });
});

// ---------------------- 9. Build app ----------------------
var app = builder.Build();

// ---------------------- 10. Middleware pipeline ----------------------
// Swagger: Always enable for local, only in Development for Docker
// CRITICAL: Swagger MUST be enabled BEFORE UseRouting() and other middleware
if (!isRunningInContainer)
{
    // Local: Always enable Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient API v1");
        c.RoutePrefix = string.Empty;
    });
}
else if (app.Environment.IsDevelopment())
{
    // Docker: Only enable in Development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient API v1");
        c.RoutePrefix = string.Empty;
    });
}

// HTTPS Redirection: DISABLED in Docker to allow HTTP access
// Only enable HTTPS redirection for local development
if (!isRunningInContainer)
{
    app.UseHttpsRedirection();
}

app.UseRouting();

// === KHÔNG CÓ UseAuthentication và UseAuthorization ===
// app.UseAuthentication(); 
// app.UseAuthorization(); 

// Note: Route "/" - Swagger UI handles it when RoutePrefix = string.Empty
// Only map "/" when Swagger is NOT enabled (Production)
if (!app.Environment.IsDevelopment())
{
    app.MapGet("/", () => "Patient Service is running");
}

app.MapControllers();

// gRPC Server
app.MapGrpcService<Patient_service.GrpcServices.PatientGrpcService>();
app.MapGet("/grpc", () => "gRPC Server is running");

// Health check endpoints
// Note: Route "/" is removed because Swagger UI uses it when RoutePrefix = string.Empty
var healthCheckEndpoint = configuration["ConsulConfig:HealthCheckEndpoint"] ?? "/health";
app.MapGet(healthCheckEndpoint, () => Results.Ok(new { status = "healthy", service = "patient-service", timestamp = DateTime.UtcNow }));

// Consul Service Registration
var consulConfig = configuration.GetSection("ConsulConfig");
// Fix: Use service name in Docker, localhost in local
var serviceHost = isRunningInContainer 
    ? (consulConfig["ServiceName"] ?? "patient-service")  // Docker: use service name
    : (consulConfig["ServiceHost"] ?? "localhost");       // Local: use localhost
// Fix: Generate unique ServiceID to avoid conflicts
var baseServiceId = consulConfig["ServiceId"] ?? "patient-service";
var serviceId = isRunningInContainer
    ? $"{baseServiceId}-{Environment.MachineName}-{DateTime.UtcNow:yyyyMMddHHmmss}"
    : $"{baseServiceId}-1"; // Local: simple ID
var serviceName = consulConfig["ServiceName"] ?? "patient-service";

var consulClient = app.Services.GetRequiredService<IConsulClient>();

var registration = new AgentServiceRegistration()
{
    ID = serviceId,
    Name = serviceName,
    Address = serviceHost,
    Port = restPort,
    Tags = new[] { "rest-api", "grpc" },
    Meta = new Dictionary<string, string>
    {
        { "grpc-port", grpcPort.ToString() },
        { "rest-port", restPort.ToString() }
    },
    Check = new AgentServiceCheck()
    {
        HTTP = $"http://{serviceHost}:{restPort}{healthCheckEndpoint}",
        Interval = TimeSpan.FromSeconds(10),
        Timeout = TimeSpan.FromSeconds(5),
        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
    }
};

//Register service to Consul with retry logic
async Task<bool> TryRegisterAsync(int attempts = 5, TimeSpan? delay = null)
{
    delay ??= TimeSpan.FromSeconds(3);
    for (int i = 0; i < attempts; i++)
    {
        try
        {
            await consulClient.Agent.ServiceRegister(registration);
            Console.WriteLine($"✅ [Consul] Service registered successfully - REST: {serviceHost}:{restPort}, gRPC: {serviceHost}:{grpcPort}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ [Consul] Attempt {i + 1}/{attempts} failed to register service: {ex.Message}");
            if (i == attempts - 1)
            {
                Console.WriteLine($"❌ [Consul] Failed to register after {attempts} attempts. Service will continue without Consul registration.");
                return false;
            }
            await Task.Delay(delay.Value);
        }
    }
    return false;
}

// Try to register, but don't block if Consul is unavailable
_ = Task.Run(async () =>
{
    try
    {
        await TryRegisterAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ [Consul] Background registration failed: {ex.Message}");
    }
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    consulClient.Agent.ServiceDeregister(serviceId).Wait();
});

app.Run();


