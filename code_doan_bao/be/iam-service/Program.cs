using Consul;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using iam_service.Data;
using iam_service.Repositories.Implement;
using iam_service.Repositories.Interface;
using iam_service.Service.Implement;
using iam_service.Service.Interface;
using IamService.Services.Implement;
using IamService.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Enable HTTP/2 without encryption for gRPC (required for local development)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

// Configuration - Environment variables take precedence over appsettings.json
var consulAddress = Environment.GetEnvironmentVariable("ConsulConfig__Address")
                    ?? builder.Configuration["ConsulConfig:Address"]
                    ?? "http://localhost:8500";

// Read ports from configuration - Environment variables take precedence
var restPort = Environment.GetEnvironmentVariable("ConsulConfig__ServicePort") != null
    ? int.Parse(Environment.GetEnvironmentVariable("ConsulConfig__ServicePort")!)
    : builder.Configuration.GetValue<int>("ConsulConfig:ServicePort", 7020);
    
var grpcPort = Environment.GetEnvironmentVariable("Grpc__Port") != null
    ? int.Parse(Environment.GetEnvironmentVariable("Grpc__Port")!)
    : builder.Configuration.GetValue<int>("Grpc:Port", 7021);

// Detect if running in container
var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// ConfigureKestrel based on environment
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
            listenOptions.Protocols = HttpProtocols.Http1;
            listenOptions.UseHttps(); // Use development certificate
        });
        
        // gRPC - HTTP/2 for gRPC communication (can use HTTP/2 without TLS for local dev)
        options.ListenAnyIP(grpcPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    }
});
// 1. Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var mongoClient = new MongoClient(connectionString);
builder.Services.AddDbContext<UserManagementDBContext>(options =>
    options.UseMongoDB(mongoClient, "UserManagementDB"));

//1.Database
// Auto-detect database host based on environment
//var dbHost = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
//    ? "postgres"
//    : "localhost";

//var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Replace host if needed (for local development)
//var connectionString = defaultConnectionString.Replace("Host=postgres", $"Host={dbHost}")
//                                               .Replace("Host=localhost", $"Host={dbHost}");

//builder.Services.AddDbContext<UserManagementDBContext>(options =>
//     options.UseNpgsql(connectionString,
//        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
//            maxRetryCount: 3,
//            maxRetryDelay: TimeSpan.FromSeconds(5),
//            errorCodesToAdd: null
//        )
//    )
//);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// 2. DI (Dependency Injection)
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IAccountRepo, AccountRepo>();


builder.Services.AddGrpc();

// 4. Swagger + JWT
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IAM Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 5. JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidateAudience = true,
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });
builder.Services.AddAuthorization();



// Consul service registration configuration
var consulConfig = builder.Configuration.GetSection("ConsulConfig");
var serviceHost = consulConfig["ServiceHost"] ?? "localhost";
var serviceId = consulConfig["ServiceId"] ?? "iam-service-1";
var serviceName = consulConfig["ServiceName"] ?? "iam-service";
var healthCheckEndpoint = consulConfig["HealthCheckEndpoint"] ?? "/health";
// Use restPort that was already read from configuration
var servicePort = restPort;

// Đăng ký Consul Client
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p =>
{
    return new ConsulClient(config =>
    {
        config.Address = new Uri(consulAddress);
    });
});


// Build app
var app = builder.Build();



// 7. Middleware
// Swagger: Always enable for local, only in Development for Docker
if (!isRunningInContainer)
{
    // Local: Always enable Swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IAM API v1");
        options.RoutePrefix = string.Empty;
    });
}
else if (app.Environment.IsDevelopment())
{
    // Docker: Only enable in Development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IAM API v1");
        options.RoutePrefix = string.Empty;
    });
}

// HTTPS Redirection: Only for local (not in container)
if (!isRunningInContainer)
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 8.Controllers

app.MapGet("/", () => "IAM Service is running");
app.MapControllers();
app.MapGrpcService<iam_service.GrpcServices.UserGrpcService>();
app.MapGet("/grpc", () => "gRPC Server is running");
app.MapGet(healthCheckEndpoint, () => Results.Ok("Healthy"));

var consulClient = app.Services.GetRequiredService<IConsulClient>();

var registration = new AgentServiceRegistration()
{
    ID = serviceId,
    Name = serviceName,
    Address = serviceHost,
    Port = servicePort, // REST API port
    Tags = new[] { "rest-api", "grpc" }, // Tags to identify service capabilities
    Meta = new Dictionary<string, string>
    {
        { "grpc-port", grpcPort.ToString() }, // Store gRPC port in metadata
        { "rest-port", servicePort.ToString() }
    },
    Check = new AgentServiceCheck()
    {
        HTTP = $"http://{serviceHost}:{servicePort}{healthCheckEndpoint}",
        Interval = TimeSpan.FromSeconds(10),
        Timeout = TimeSpan.FromSeconds(5),
        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
    }
};

// Register service to Consul with retry logic
async Task<bool> TryRegisterAsync(int attempts = 5, TimeSpan? delay = null)
{
    delay ??= TimeSpan.FromSeconds(3);
    for (int i = 0; i < attempts; i++)
    {
        try
        {
            await consulClient.Agent.ServiceRegister(registration);
            Console.WriteLine($"✅ [Consul] Service registered successfully - REST: {serviceHost}:{servicePort}, gRPC: {serviceHost}:{grpcPort}");
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
