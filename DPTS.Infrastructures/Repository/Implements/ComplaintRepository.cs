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

        #region Query Builder
        private IQueryable<Complaint> BuildBaseQuery(
            bool includeProduct = false,
            bool includeUser = false,
            bool includeOrder = false,
            bool includeImages = false)
        {
            var query = _context.Complaints.AsQueryable();

            if (includeProduct)
                query = query.Include(c => c.Product);

            if (includeUser)
                query = query.Include(c => c.User);

            if (includeOrder)
                query = query.Include(c => c.Order);

            if (includeImages)
                query = query.Include(c => c.Images);

            return query;
        }
        #endregion

        #region Gets
        public async Task<IEnumerable<Complaint>> GetAllAsync(
            ComplaintStatus? status = null,
            string? userId = null,
            bool includeProduct = false,
            bool includeUser = false,
            bool includeOrder = false,
            bool includeImages = false)
        {
            var query = BuildBaseQuery(includeProduct, includeUser, includeOrder, includeImages);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(c => c.UserId == userId);

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Complaint?> GetByIdAsync(
            string complaintId,
            bool includeProduct = false,
            bool includeUser = false,
            bool includeOrder = false,
            bool includeImages = false)
        {
            if (string.IsNullOrWhiteSpace(complaintId))
                return null;

            return await BuildBaseQuery(includeProduct, includeUser, includeOrder, includeImages)
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId);
        }

        public async Task<IEnumerable<Complaint>> GetByOrderIdAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return Enumerable.Empty<Complaint>();

            return await _context.Complaints
                .Where(c => c.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string complaintId)
        {
            return await _context.Complaints.AnyAsync(c => c.ComplaintId == complaintId);
        }
        #endregion

        #region CRUD
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

        public async Task DeleteAsync(string complaintId)
        {
            var complaint = await _context.Complaints.FindAsync(complaintId);
            if (complaint != null)
            {
                _context.Complaints.Remove(complaint);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
    }
}
