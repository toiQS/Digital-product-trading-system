using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class ComplaintImageRepository : IComplaintImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ComplaintImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ComplaintImage>> GetsAsync(
            string? complaintId = null,
            string? keyword = null,
            bool includeComplaint = false)
        {
            var query = _context.ComplaintImages.AsQueryable();

            // Ưu tiên lọc theo complaintId nếu có
            if (!string.IsNullOrWhiteSpace(complaintId))
                query = query.Where(i => i.ComplaintId == complaintId);

            // Include Complaint nếu cần
            if (includeComplaint)
                query = query.Include(i => i.Complaint);

            // Tìm theo từ khóa (nếu có)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var pattern = $"%{keyword}%";
                query = query.Where(i =>
                    EF.Functions.Like(i.ImagePath, pattern));
            }

            return await query.ToListAsync();
        }

        public async Task<ComplaintImage?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            return await _context.ComplaintImages
                .Include(i => i.Complaint)
                .FirstOrDefaultAsync(i => i.ImageId == id);
        }

        public async Task<IEnumerable<ComplaintImage>> GetByComplaintIdAsync(string complaintId)
        {
            if (string.IsNullOrWhiteSpace(complaintId)) return Enumerable.Empty<ComplaintImage>();

            return await _context.ComplaintImages
                .Where(i => i.ComplaintId == complaintId)
                .ToListAsync();
        }

        public async Task AddAsync(ComplaintImage image)
        {
            _context.ComplaintImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var image = await _context.ComplaintImages.FindAsync(id);
            if (image != null)
            {
                _context.ComplaintImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
