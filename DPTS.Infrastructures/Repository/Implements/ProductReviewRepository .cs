using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReview>> GetsAsync(
            string? search = null,
            string? productId = null,
            string? userId = null,
            int? minRating = null,
            int? maxRating = null,
            DateTime? from = null,
            DateTime? to = null,
            bool includeUser = false,
            bool includeProduct = false)
        {
            var query = _context.ProductReviews.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLower();
                query = query.Where(r =>
                    r.Comment.ToLower().Contains(lowered) ||
                    r.ProductId.ToLower().Contains(lowered) ||
                    r.UserId.ToLower().Contains(lowered));
            }

            if (!string.IsNullOrWhiteSpace(productId))
                query = query.Where(r => r.ProductId == productId);

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(r => r.UserId == userId);

            if (minRating.HasValue)
                query = query.Where(r => r.Rating >= minRating.Value);

            if (maxRating.HasValue)
                query = query.Where(r => r.Rating <= maxRating.Value);

            if (from.HasValue)
                query = query.Where(r => r.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(r => r.CreatedAt <= to.Value);

            if (includeUser)
                query = query.Include(r => r.User);

            if (includeProduct)
                query = query.Include(r => r.Product);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<ProductReview?> GetByIdAsync(string reviewId, bool includeUser = false, bool includeProduct = false)
        {
            if (string.IsNullOrWhiteSpace(reviewId))
                return null;

            var query = _context.ProductReviews.AsQueryable();

            if (includeUser)
                query = query.Include(r => r.User);

            if (includeProduct)
                query = query.Include(r => r.Product);

            return await query.FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task AddAsync(ProductReview review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            review.CreatedAt = DateTime.UtcNow;
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductReview review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string reviewId)
        {
            if (string.IsNullOrWhiteSpace(reviewId))
                return;

            var review = await _context.ProductReviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.ProductReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }

}