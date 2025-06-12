using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductImage>> GetsAsync(
            string? search = null,
            bool? isPrimary = null,
            DateTime? from = null,
            DateTime? to = null,
            string? productId = null,
            bool includeProduct = false)
        {
            var query = _context.ProductImages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLower();
                query = query.Where(i =>
                    i.ImagePath.ToLower().Contains(lowered) ||
                    i.ProductId.ToLower().Contains(lowered));
            }

            if (!string.IsNullOrWhiteSpace(productId))
            {
                query = query.Where(i => i.ProductId == productId);
            }

            if (isPrimary.HasValue)
            {
                query = query.Where(i => i.IsPrimary == isPrimary.Value);
            }

            if (from.HasValue)
            {
                query = query.Where(i => i.CreatedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(i => i.CreatedAt <= to.Value);
            }

            if (includeProduct)
            {
                query = query.Include(i => i.Product);
            }

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductImage?> GetByIdAsync(string id, bool includeProduct = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var query = _context.ProductImages.AsQueryable();

            if (includeProduct)
                query = query.Include(i => i.Product);

            return await query
                .FirstOrDefaultAsync(i => i.ImageId == id);
        }

        public async Task AddAsync(ProductImage image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            image.CreatedAt = DateTime.UtcNow;
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductImage image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            _context.ProductImages.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return;

            var image = await _context.ProductImages.FindAsync(id);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }

}