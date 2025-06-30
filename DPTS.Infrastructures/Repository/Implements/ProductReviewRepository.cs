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

        #region Create / Update / Delete

        public async Task AddAsync(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductReview review)
        {
            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string reviewId)
        {
            var review = await _context.ProductReviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.ProductReviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Read

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(string productId, int skip = 0, int take = 10)
        {
            var query = _context.ProductReviews.Where(x => x.ProductId == productId).AsQueryable();
            if(skip > 0 && take > 0)
            {
                query = query.Skip(skip).Take(take);
            }
            return await query.ToListAsync();
        }

        public async Task<ProductReview?> GetByUserAndProductAsync(string userId, string productId)
        {
            return await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<double> GetAverageOverallRatingAsync(string productId)
        {
            return await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .AverageAsync(r => r.RatingOverall);
        }

        public async Task<int> CountByProductIdAsync(string productId)
        {
            return await _context.ProductReviews
                .CountAsync(r => r.ProductId == productId);
        }

        public async Task<IEnumerable<ProductReview>> GetTopLikedReviewsAsync(string productId, int take = 3)
        {
            return await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Likes)
                .ThenByDescending(r => r.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        #endregion
    }
}
