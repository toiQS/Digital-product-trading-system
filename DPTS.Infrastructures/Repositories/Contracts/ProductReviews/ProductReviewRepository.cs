
using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductReviews
{
    public class ProductReviewRepository : IProductReviewQuery, IProductReviewCommand
    {
        private readonly ApplicationDbContext _context;
        public ProductReviewRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Task<double> GetAverageRatingByProductIdAsync(string productId, CancellationToken cancellationToken)
        {
            return _context.ProductReviews
                .Where(pr => pr.ProductId == productId)
                .AverageAsync(pr => pr.RatingOverall, cancellationToken);
        }

        public async Task<int> GetCountRatingByProductIdAsync(string productId, CancellationToken cancellationToken)
        {
            return await _context.ProductReviews
                .Where(pr => pr.ProductId == productId)
                .CountAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetPositiveFeedbacksAsync(int take, CancellationToken cancellationToken)
        {
            var query = _context.ProductReviews.OrderByDescending(pr => pr.RatingOverall).AsQueryable();
            if (take > 0)
            {
                query = query.Take(take);
            }
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProductReview>> GetsByProductIdAsync(string productId, CancellationToken cancellationToken)
        {
            return await _context.ProductReviews
                .Where(pr => pr.ProductId == productId)
                .ToListAsync(cancellationToken);
        }
    }
}
