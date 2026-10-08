namespace Testorder_service.Services.Interface
{
    public interface IEmailNotificationService
    {
        /// <summary>
        /// Gửi email khi TestResult cập nhật trạng thái thành "waiting review"
        /// </summary>
        Task SendTestResultWaitingReviewEmailAsync(string patientEmail, string testResultId, string appointmentId, string testPackageName);

        /// <summary>
        /// Gửi email khi Appointment được đặt lịch (status = "scheduled")
        /// </summary>
        Task SendAppointmentScheduledEmailAsync(string patientEmail, string appointmentId, string testPackageName, 
            DateTime appointmentDate, string doctorName, string roomName);
    }
}

