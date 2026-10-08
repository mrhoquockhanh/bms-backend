using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using CacheManager.Core;
using Ocelot.Provider.Consul;

namespace api_gateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔹 Load Ocelot config
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            // 🔹 Add Controllers
            builder.Services.AddControllers();

            // 🔹 Add Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 🔹 JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                            ?? jwtSettings["SecretKey"];

            Console.WriteLine($"[DEBUG] Loaded JWT Settings:");
            Console.WriteLine($"[DEBUG] Issuer: {jwtSettings["Issuer"]}");
            Console.WriteLine($"[DEBUG] Audience: {jwtSettings["Audience"]}");
            Console.WriteLine($"[DEBUG] SecretKey: {secretKey?.Substring(0, 5)}...");

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            // ⚠️ FIX: Detect and replace placeholders from appsettings.Development.json
            if (issuer == "your-issuer") 
            {
                Console.WriteLine("[WARNING] Placeholder Issuer detected. Forcing 'IamService'.");
                issuer = "IamService";
            }
            if (audience == "your-audience") 
            {
                Console.WriteLine("[WARNING] Placeholder Audience detected. Forcing 'ApiGateway'.");
                audience = "ApiGateway";
            }
            if (secretKey == "fallback-secret-if-env-not-set") 
            {
                Console.WriteLine("[WARNING] Placeholder SecretKey detected. Forcing correct key.");
                secretKey = "MotChuoiKhoaBiMatRatDaiVaBaoMatHon32KyTu";
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                });

            // 🔹 CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost3000",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000", "https://localhost:3000") // cho phép cả HTTP & HTTPS
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            // 🔹 Data Protection
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys"))
                .SetApplicationName("ApiGateway");

            // 🔹 Ocelot + Polly + Consul
            builder.Services.AddOcelot()
                .AddPolly()
                .AddConsul();

            var app = builder.Build();

            // 🔹 Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowLocalhost3000"); // ⚠️ THÊM DÒNG NÀY

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.UseOcelot(); // ⚠️ PHẢI ĐỂ SAU UseCors()

            app.Run();
        }
    }
}
