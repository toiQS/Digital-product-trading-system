using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly ApplicationDbContext _context;

        public ComplaintRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Complaint>> GetsAsync(
            string? userId = null,
            string? productId = null,
            string? orderId = null,
            string? text = null,
            ComplaintStatus? status = null,
            bool includeUser = false,
            bool includeOrder = false,
            bool includeProduct = false,
            bool includeImages = false)
        {
            var query = _context.Complaints.AsQueryable();

            // Filtering by ID fields
            if (!string.IsNullOrWhiteSpace(userId)) query = query.Where(x => x.UserId == userId);
            if (!string.IsNullOrWhiteSpace(productId)) query = query.Where(x => x.ProductId == productId);
            if (!string.IsNullOrWhiteSpace(orderId)) query = query.Where(x => x.OrderId == orderId);

            // Conditional includes
            if (includeUser) query = query.Include(x => x.User);
            if (includeOrder) query = query.Include(x => x.Order);
            if (includeProduct) query = query.Include(x => x.Product);
            if (includeImages) query = query.Include(x => x.Images);

            // Text search
            if (!string.IsNullOrWhiteSpace(text))
            {
                var pattern = $"%{text}%";
                query = query.Where(c =>
                    EF.Functions.Like(c.Title, pattern) ||
                    EF.Functions.Like(c.Description, pattern) ||
                    EF.Functions.Like(c.UserId, pattern) ||
                    EF.Functions.Like(c.OrderId, pattern) ||
                    EF.Functions.Like(c.ProductId, pattern));
            }

            // Filter by status
            if (status.HasValue) query = query.Where(x => x.Status == status);

            return await query.ToListAsync();
        }

        public async Task<Complaint?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            return await _context.Complaints
                .Include(x => x.User)
                .Include(x => x.Order)
                .Include(x => x.Product)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.ComplaintId == id);
        }

        public async Task<IEnumerable<Complaint>> GetByUserIdAsync(string userId)
        {
            return await _context.Complaints
                .Where(x => x.UserId == userId)
                .Include(x => x.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Complaint>> GetByOrderIdAsync(string orderId)
        {
            return await _context.Complaints
                .Where(x => x.OrderId == orderId)
                .Include(x => x.Images)
                .ToListAsync();
        }

        public async Task AddAsync(Complaint complaint)
        {
            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Complaint complaint)
        {
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint != null)
            {
                _context.Complaints.Remove(complaint);
                await _context.SaveChangesAsync();
            }
        }
    }
}
