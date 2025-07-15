using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

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
        
        public async Task<bool> ExistsAsync(string complaintId)
        {
            return await _context.Complaints.AnyAsync(c => c.ComplaintId == complaintId);
        }

        public async Task<IEnumerable<Complaint>> GetAllAsync(ComplaintStatus? status = null,
                                                              string? userId = null,
                                                              bool includeProduct = false,
                                                              bool includeUser = false,
                                                              bool includeEscrow = false,
                                                              bool includeImages = false)
        {
            var query = _context.Complaints.AsQueryable();
            if (status != null)
            {
                query = query.Where(x => x.Status == status);
            }
            if (userId != null)
            {
                query = query.Where(x => x.UserId == userId);
            }
            if (includeEscrow)
            {
                query = query.Include(x => x.Escrow);
            }
            if (includeImages)
            {
                query = query.Include(x => x.Images);
            }
            if (includeProduct)
            {
                query = query.Include(x => x.Product);
            }
            if (includeUser)
            {
                query = query.Include(x => x.User);
            }
            return await query.ToListAsync();
        }

        public async Task<Complaint?> GetByIdAsync(string complaintId, bool includeProduct = false, bool includeUser = false, bool includeEscrow = false, bool includeImages = false)
        {
            var query = _context.Complaints.AsQueryable();
            if (!string.IsNullOrEmpty(complaintId))
            {
                query = query.Where(x => x.ComplaintId == complaintId);
            }
            if (includeEscrow)
            {
                query = query.Include(x => x.Escrow);
            }
            if (includeImages)
            {
                query = query.Include(x => x.Images);
            }
            if (includeProduct)
            {
                query = query.Include(x => x.Product);
            }
            if (includeUser)
            {
                query = query.Include(x => x.User);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Complaint>> GetByEscrowIdAsync(string escrowId)
        {
            return await _context.Complaints.Where(x => x.EscrowId == escrowId).ToListAsync();
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

        public async Task<IEnumerable<Complaint>> GetByProductIdAsync(string productId)
        {
            return await _context.Complaints.Where(x => x.ProductId == productId).ToListAsync();
        }


        #endregion
    }
}
