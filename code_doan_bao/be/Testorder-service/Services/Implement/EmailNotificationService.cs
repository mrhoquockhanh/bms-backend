using Testorder_service.GrpcClients;
using Testorder_service.Services.Interface;

namespace Testorder_service.Services.Implement
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly IamGrpcClient _iamGrpcClient;

        public EmailNotificationService(
            ILogger<EmailNotificationService> logger,
            IamGrpcClient iamGrpcClient)
        {
            _logger = logger;
            _iamGrpcClient = iamGrpcClient;
        }

        public Task SendTestResultWaitingReviewEmailAsync(
            string patientEmail, 
            string testResultId, 
            string appointmentId, 
            string testPackageName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(patientEmail))
                {
                    _logger.LogWarning("⚠️ Cannot send email: Patient email is empty");
                    return Task.CompletedTask;
                }

                // Email notification functionality removed - SQS/SNS has been removed
                _logger.LogInformation("ℹ️ Email notification skipped for TestResult {TestResultId} - Waiting Review (SQS/SNS removed)", testResultId);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to publish email notification event for TestResult {TestResultId} - Waiting Review", testResultId);
                // Don't throw - email failure shouldn't break the main flow
                return Task.CompletedTask;
            }
        }

        public Task SendAppointmentScheduledEmailAsync(
            string patientEmail, 
            string appointmentId, 
            string testPackageName, 
            DateTime appointmentDate, 
            string doctorName, 
            string roomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(patientEmail))
                {
                    _logger.LogWarning("⚠️ Cannot send email: Patient email is empty");
                    return Task.CompletedTask;
                }

                // Email notification functionality removed - SQS/SNS has been removed
                _logger.LogInformation("ℹ️ Email notification skipped for Appointment {AppointmentId} - Scheduled (SQS/SNS removed)", appointmentId);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to publish email notification event for Appointment {AppointmentId} - Scheduled", appointmentId);
                // Don't throw - email failure shouldn't break the main flow
                return Task.CompletedTask;
            }
        }
    }
}

