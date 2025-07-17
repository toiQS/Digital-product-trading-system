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

        #region Create / Update / Delete

        public async Task AddAsync(ProductImage image)
        {
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ProductImage> images)
        {
            _context.ProductImages.AddRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByProductIdAsync(string productId)
        {
            var images = await _context.ProductImages
                .Where(p => p.ProductId == productId)
                .ToListAsync();

            _context.ProductImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task SetPrimaryAsync(string productId, string imageId)
        {
            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .ToListAsync();

            foreach (var img in images)
                img.IsPrimary = (img.ImageId == imageId);

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(string productId)
        {
            return await _context.ProductImages
                .Where(p => p.ProductId == productId)
                .OrderByDescending(p => p.IsPrimary)
                .ThenBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductImage?> GetPrimaryAsync(string productId)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsPrimary);
        }

        public async Task<ProductImage?> GetByIdAsync(string imageId)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(p => p.ImageId == imageId);
        }

        public async Task UpdateAsync(ProductImage primaryImage)
        {
            _context.ProductImages.Update(primaryImage);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
