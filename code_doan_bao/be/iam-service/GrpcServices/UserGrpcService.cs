using Grpc.Core;
using GrpcAuth_Server;
using iam_service.Modals;
using iam_service.Repositories.Interface;
using IamService.Services.Interface;
using System.Security.Claims;

namespace iam_service.GrpcServices
{
    public class UserGrpcService : UserService.UserServiceBase
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserGrpcService> _logger;

        public UserGrpcService(IAccountRepo accountRepo, IJwtService jwtService, ILogger<UserGrpcService> logger)
        {
            _accountRepo = accountRepo;
            _jwtService = jwtService;
            _logger = logger;
        }

        public override async Task<GetUserResponse> GetUserById(GetUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC GetUserById: {UserId}", request.UserId);

            var user = await _accountRepo.GetByIdAsync(request.UserId);
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User {request.UserId} not found"));

            return MapToGrpcUser(user);
        }

        public override async Task<GetUserResponse> GetUserByEmail(GetUserByEmailRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC GetUserByEmail: {Email}", request.Email);

            var user = await _accountRepo.GetByEmailAsync(request.Email);
            if (user == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"User with email {request.Email} not found"));

            return MapToGrpcUser(user);
        }

        public override Task<ValidateTokenResponse> ValidateUserToken(ValidateTokenRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC ValidateUserToken");

            var principal = _jwtService.ValidateToken(request.Token);
            if (principal == null)
            {
                return Task.FromResult(new ValidateTokenResponse { Valid = false, UserId = string.Empty });
            }

            // Tìm nhiều kiểu claim phổ biến (sub, nameid, userId)
            string? userId =
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal.FindFirst("sub")?.Value
                ?? principal.FindFirst("userId")?.Value
                ?? principal.FindFirst("UserId")?.Value;

            userId ??= string.Empty;

            return Task.FromResult(new ValidateTokenResponse { Valid = true, UserId = userId });
        }

        private static GetUserResponse MapToGrpcUser(Account user)
        {
            return new GetUserResponse
            {
                UserId = user.Id,
                UserName = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                Status = user.Status,
                CreatedAt = user.CreatedAt.ToString("o") ?? string.Empty
            };
        }
    }
}
