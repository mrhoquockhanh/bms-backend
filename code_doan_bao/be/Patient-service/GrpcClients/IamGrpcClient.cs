using Consul;
using Grpc.Net.Client;
using GrpcAuth_Server;

namespace Patient_service.GrpcClients
{
    public class IamGrpcClient
    {
        private readonly IConsulClient _consulClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IamGrpcClient> _logger;

        // gRPC port is fixed at 7021, not the REST port from Consul
        private const int GrpcPort = 7021;

        public IamGrpcClient(IConsulClient consulClient, IConfiguration configuration, ILogger<IamGrpcClient> logger)
        {
            _consulClient = consulClient;
            _configuration = configuration;
            _logger = logger;
        }

        private async Task<(string address, int port)> GetIamServiceAddressAndPortAsync()
        {
            // Declare variables at method scope to avoid conflicts
            string address;
            int grpcPort = GrpcPort;
            var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            try
            {
                // Try to get from Consul first
                var services = await _consulClient.Agent.Services();
                var iamService = services.Response.Values.FirstOrDefault(s => s.Service == "iam-service");

                if (iamService != null)
                {
                    // Use address from Consul (works for both Docker and local)
                    address = iamService.Address;
                    if (string.IsNullOrEmpty(address) || address == "0.0.0.0")
                    {
                        // Fix: Use service name in Docker, localhost in local
                        address = isRunningInContainer ? "iam-service" : "localhost";
                    }
                    
                    // Try to get gRPC port from Consul metadata, fallback to default
                    grpcPort = GrpcPort;
                    if (iamService.Meta != null && iamService.Meta.TryGetValue("grpc-port", out var grpcPortStr))
                    {
                        if (int.TryParse(grpcPortStr, out var parsedPort))
                        {
                            grpcPort = parsedPort;
                            _logger.LogInformation("🔗 Found IAMService in Consul: {Address} (gRPC port from metadata: {Port})", address, grpcPort);
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ Invalid gRPC port in Consul metadata: {PortStr}, using default: {DefaultPort}", grpcPortStr, GrpcPort);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("🔗 Found IAMService in Consul: {Address} (gRPC port: {Port})", address, grpcPort);
                    }
                    
                    return (address, grpcPort);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("⚠️ Failed to get service from Consul: {Error}. Using fallback.", ex.Message);
            }

            // Fallback to configuration or service name/localhost based on environment
            var fallbackUrl = _configuration["GrpcSettings:IamServiceUrl"];
            
            if (!string.IsNullOrEmpty(fallbackUrl))
            {
                var fallbackAddress = fallbackUrl.Replace("http://", "").Replace("https://", "");
                if (fallbackAddress.Contains(":"))
                {
                    var parts = fallbackAddress.Split(':');
                    address = parts[0];
                    if (parts.Length > 1 && int.TryParse(parts[1], out var parsedPort))
                    {
                        grpcPort = parsedPort;
                    }
                }
                else
                {
                    address = fallbackAddress;
                }
                address = address ?? (isRunningInContainer ? "iam-service" : "localhost");
            }
            else
            {
                // Final fallback: service name in Docker, localhost in local
                address = isRunningInContainer ? "iam-service" : "localhost";
            }
            
            return (address, grpcPort);
        }

        private GrpcChannel CreateChannel(string address, int port)
        {
            var grpcAddress = $"http://{address}:{port}";
            _logger.LogInformation("🔗 Creating gRPC channel to: {Address}", grpcAddress);
            
            return GrpcChannel.ForAddress(grpcAddress, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }
            });
        }

        public async Task<GetUserResponse?> GetUserByIdAsync(string userId)
        {
            var (address, port) = await GetIamServiceAddressAndPortAsync();
            using var channel = CreateChannel(address, port);
            var client = new UserService.UserServiceClient(channel);

            try
            {
                _logger.LogInformation("📞 Calling gRPC GetUserById: {UserId}", userId);
                var response = await client.GetUserByIdAsync(new GetUserRequest { UserId = userId });
                _logger.LogInformation("✅ gRPC GetUserById successful: {Email}", response.Email);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ gRPC GetUserById failed for userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<GetUserResponse?> GetUserByEmailAsync(string email)
        {
            var (address, port) = await GetIamServiceAddressAndPortAsync();
            using var channel = CreateChannel(address, port);
            var client = new UserService.UserServiceClient(channel);

            try
            {
                _logger.LogInformation("📞 Calling gRPC GetUserByEmail: {Email}", email);
                var response = await client.GetUserByEmailAsync(new GetUserByEmailRequest { Email = email });
                _logger.LogInformation("✅ gRPC GetUserByEmail successful: {UserId}", response.UserId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ gRPC GetUserByEmail failed for email: {Email}", email);
                throw;
            }
        }

        public async Task<ValidateTokenResponse?> ValidateUserTokenAsync(string token)
        {
            var (address, port) = await GetIamServiceAddressAndPortAsync();
            using var channel = CreateChannel(address, port);
            var client = new UserService.UserServiceClient(channel);

            try
            {
                _logger.LogInformation("📞 Calling gRPC ValidateUserToken");
                var response = await client.ValidateUserTokenAsync(new ValidateTokenRequest { Token = token });
                _logger.LogInformation("✅ gRPC ValidateUserToken successful: Valid={Valid}, UserId={UserId}", 
                    response.Valid, response.UserId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ gRPC ValidateUserToken failed");
                throw;
            }
        }
    }
}
