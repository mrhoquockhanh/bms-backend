using TestOrder.Models.DTO;
using TestOrder.Repositories.Interface;
using TestOrder.Services.Interface;
using Testorder_service.Models.Entities;

namespace TestOrder.Services.Implement
{
    public class TestOrderCommentService : ITestOrderCommentService
    {
        private readonly ITestOrderCommentRepository _repo;

        public TestOrderCommentService(ITestOrderCommentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TestOrderCommentDto>> GetAllAsync()
        {
            var comments = await _repo.GetAllAsync();
            return comments.Select(c => new TestOrderCommentDto
            {
                CommentId = c.CommentId,
                AppointmentId = c.AppointmentId,
                CommentText = c.CommentText,
                CreatedBy = c.CreatedBy,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<TestOrderCommentDto?> GetByIdAsync(string id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;

            return new TestOrderCommentDto
            {
                CommentId = c.CommentId,
                AppointmentId = c.AppointmentId,
                CommentText = c.CommentText,
                CreatedBy = c.CreatedBy,
                CreatedAt = c.CreatedAt
            };
        }

        public async Task<IEnumerable<TestOrderCommentDto>> GetByAppointmentIdAsync(string appointmentId)
        {
            var comments = await _repo.GetByAppointmentIdAsync(appointmentId);
            return comments.Select(c => new TestOrderCommentDto
            {
                CommentId = c.CommentId,
                AppointmentId = c.AppointmentId,
                CommentText = c.CommentText,
                CreatedBy = c.CreatedBy,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task CreateAsync(CreateTestOrderCommentDto dto)
        {
            var random = new Random();
            var comment = new TestOrderComment
            {
                CommentId = $"COM{random.Next(0, 999999):D6}",
                AppointmentId = dto.AppointmentId,
                CommentText = dto.CommentText,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(comment);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, UpdateTestOrderCommentDto dto)
        {
            var comment = await _repo.GetByIdAsync(id);
            if (comment == null) throw new KeyNotFoundException("Comment not found.");

            comment.CommentText = dto.CommentText ?? comment.CommentText;

            await _repo.UpdateAsync(comment);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }
    }
}
