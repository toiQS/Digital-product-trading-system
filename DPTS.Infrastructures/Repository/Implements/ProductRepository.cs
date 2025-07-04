﻿using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetsAsync(
            string? storeId = null,
            string? text = null,
            string? categoryId = null,
            ProductStatus? status = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? keyword = null,
            bool includeCategory = false,
            bool includeImages = false,
            bool includeReviews = false,
            bool includeStore = false,
            bool includeOrderItem = false)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(storeId))
                query = query.Where(p => p.StoreId == storeId);

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(p => EF.Functions.Like(p.ProductName, $"%{text}%"));

            if (!string.IsNullOrWhiteSpace(categoryId))
                query = query.Where(p => p.CategoryId == categoryId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.OriginalPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.OriginalPrice <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.ProductName.Contains(keyword));

            if (includeCategory)
                query = query.Include(p => p.Category);

            if (includeImages)
                query = query.Include(p => p.Images);

            if (includeReviews)
                query = query.Include(p => p.Reviews);

            if(includeStore)
                query = query.Include(p=> p.Store);

            if (includeOrderItem)
                query = query.Include(p => p.OrderItems);

            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(
            string id,
            bool includeCategory = false,
            bool includeImages = false,
            bool includeReviews = false,
            bool includeStore = false,
            bool includeOrderItem = false)
        {
            var query = _context.Products
                .Where(p => p.ProductId == id)
                .AsQueryable();

            if (includeCategory)
                query = query.Include(p => p.Category);

            if (includeImages)
                query = query.Include(p => p.Images);

            if (includeReviews)
                query = query.Include(p => p.Reviews);

            if (includeStore)
                query = query.Include(p => p.Store);

            if (includeOrderItem)
                query = query.Include(p => p.OrderItems);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
