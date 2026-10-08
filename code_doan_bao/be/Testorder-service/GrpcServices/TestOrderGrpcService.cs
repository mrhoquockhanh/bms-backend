using Grpc.Core;
using GrpcTestOrder_Server;
using TestOrder.Models.DTO;
using TestOrder.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Testorder_service.GrpcServices
{
    public class TestOrderGrpcService : TestOrderService.TestOrderServiceBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<TestOrderGrpcService> _logger;

        public TestOrderGrpcService(IAppointmentService appointmentService, ILogger<TestOrderGrpcService> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC CreateOrder: TestPackageId={TestPackageId}, CreatedBy={CreatedBy}", 
                request.TestPackageId, request.CreatedBy);

            try
            {
                // Create appointment DTO from gRPC request
                var appointmentDto = new AppointmentCreateDto
                {
                    TestPackageId = string.IsNullOrEmpty(request.TestPackageId) ? null : request.TestPackageId,
                    DoctorShiftId = string.IsNullOrEmpty(request.DoctorShiftId) ? null : request.DoctorShiftId,
                    Status = request.Status ?? "Pending",
                    CreatedBy = request.CreatedBy,
                    LabAssignmentId = string.IsNullOrEmpty(request.LabAssignmentId) ? null : request.LabAssignmentId
                };

                // Create appointment using service
                var appointment = await _appointmentService.CreateAsync(appointmentDto);

                if (appointment == null)
                {
                    _logger.LogWarning("Failed to create appointment");
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to create order"));
                }

                var response = new CreateOrderResponse
                {
                    OrderId = appointment.AppointmentId,
                    TestPackageId = appointment.TestPackageId ?? string.Empty,
                    DoctorShiftId = appointment.DoctorShiftId ?? string.Empty,
                    Status = appointment.Status ?? "Pending",
                    CreatedAt = appointment.CreatedAt?.ToString("o") ?? DateTime.UtcNow.ToString("o"),
                    Success = true,
                    Message = "Order created successfully"
                };

                _logger.LogInformation("✅ gRPC CreateOrder successful: OrderId={OrderId}", appointment.AppointmentId);
                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in CreateOrder");
                throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {ex.Message}"));
            }
        }
    }
}

