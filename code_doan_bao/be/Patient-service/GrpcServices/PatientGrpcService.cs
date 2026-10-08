using Grpc.Core;
using GrpcPatient_Server;
using Patient_service.Repositories.Interface;
using Microsoft.Extensions.Logging;

namespace Patient_service.GrpcServices
{
    public class PatientGrpcService : PatientService.PatientServiceBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientGrpcService> _logger;

        public PatientGrpcService(IPatientRepository patientRepository, ILogger<PatientGrpcService> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public override async Task<GetPatientResponse> GetPatientById(GetPatientRequest request, ServerCallContext context)
        {
            _logger.LogInformation("gRPC GetPatientById: {PatientId}", request.PatientId);

            try
            {
                var patient = await _patientRepository.GetByIdAsync(request.PatientId);
                
                if (patient == null)
                {
                    _logger.LogWarning("Patient not found: {PatientId}", request.PatientId);
                    throw new RpcException(new Status(StatusCode.NotFound, $"Patient {request.PatientId} not found"));
                }

                var response = new GetPatientResponse
                {
                    PatientId = patient.Id,
                    Fullname = patient.Fullname ?? string.Empty,
                    Dob = patient.Dob?.ToString("yyyy-MM-dd") ?? string.Empty,
                    Gender = patient.Gender ?? string.Empty,
                    Phone = patient.Phone ?? string.Empty,
                    Email = patient.Email ?? string.Empty,
                    Address = patient.Address ?? string.Empty,
                    BloodType = patient.BloodType ?? string.Empty,
                    IdentityNumber = patient.IdentityNumber ?? string.Empty,
                    CreatedAt = patient.CreatedAt?.ToString("o") ?? string.Empty,
                    UpdatedAt = patient.UpdatedAt?.ToString("o") ?? string.Empty,
                    MedicalHistory = patient.Medical_History ?? string.Empty,
                    CreatedBy = patient.Createby ?? string.Empty
                };

                _logger.LogInformation("✅ gRPC GetPatientById successful: {Fullname}", patient.Fullname);
                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetPatientById for PatientId: {PatientId}", request.PatientId);
                throw new RpcException(new Status(StatusCode.Internal, $"Internal error: {ex.Message}"));
            }
        }
    }
}

