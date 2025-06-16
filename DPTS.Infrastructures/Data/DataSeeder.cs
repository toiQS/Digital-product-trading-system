using DPTS.Domains;
using Microsoft.Extensions.DependencyInjection;

namespace DPTS.Infrastructures.Data;

public static class DataSeeder
{
    public static async Task SeedAllAsync(this IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Users.Any()) return;

        // Seed Roles
        var roles = new[]
        {
            new Role { RoleId = Guid.NewGuid().ToString(), RoleName = "Admin", Description = "Admin" },
            new Role { RoleId = Guid.NewGuid().ToString(), RoleName = "Buyer", Description = "Buyer" },
            new Role { RoleId = Guid.NewGuid().ToString(), RoleName = "Seller", Description = "Seller" }
        };
        await context.Roles.AddRangeAsync(roles);

        // Seed Users
        var admin = CreateUser("admin", "admin@gmail.com", roles[0].RoleId);
        var buyer = CreateUser("buyer", "buyer@gmail.com", roles[1].RoleId);
        var seller = CreateUser("seller", "seller@gmail.com", roles[2].RoleId);
        await context.Users.AddRangeAsync(admin, buyer, seller);

        // Wallets
        var adminWallet = CreateWallet(admin.UserId);
        var buyerWallet = CreateWallet(buyer.UserId);
        var sellerWallet = CreateWallet(seller.UserId);
        await context.Wallets.AddRangeAsync(adminWallet, buyerWallet, sellerWallet);
        // Category
        var category = new Category
        {
            CategoryName = "Electronics",
            CategoryIcon = "icon.png",
            CategoryId = "electronics",
            CreateAt = DateTime.UtcNow,
        };
        await context.Categories.AddAsync(category);

        // Products
        var productList = new List<Product>();
        for (int i = 1; i <= 3; i++)
        {
            var p = new Product
            {
                ProductId = Guid.NewGuid().ToString(),
                ProductName = $"Sample Product {i}",
                Description = $"Description for product {i}",
                Price = 100000 * i, // 100k, 200k, 300k
                SellerId = seller.UserId,
                CategoryId = category.CategoryId,
                Status = ProductStatus.Available,
                CreatedAt = DateTime.UtcNow.AddDays(-i), // giả lập ngày khác nhau
                UpdatedAt = DateTime.UtcNow,
            };
            productList.Add(p);
        }
        await context.Products.AddRangeAsync(productList);

        // ProductImages
        foreach (var p in productList)
        {
            await context.ProductImages.AddAsync(new ProductImage
            {
                ImageId = Guid.NewGuid().ToString(),
                ProductId = p.ProductId,
                ImagePath = $"/images/{p.ProductName.Replace(" ", "_").ToLower()}.jpg",
                IsPrimary = true,
            });
        }

        // ProductReviews
        foreach (var p in productList)
        {
            await context.ProductReviews.AddAsync(new ProductReview
            {
                ReviewId = Guid.NewGuid().ToString(),
                ProductId = p.ProductId,
                UserId = buyer.UserId,
                Rating = 4 + (productList.IndexOf(p) % 2), // 4 or 5
                Comment = $"Good quality for {p.ProductName}",
            });
        }

        // Orders
        for (int j = 1; j <= 2; j++)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                BuyerId = buyer.UserId,
                TotalAmount = 0, // sẽ cộng phía dưới
                IsPaied = true,
            };
            await context.Orders.AddAsync(order);

            // Add 2 items per order
            for (int k = 0; k < 2; k++)
            {
                var product = productList[(j + k - 1) % productList.Count];
                var quantity = (k + 1) * j; // 1,2 or 2,4
                var itemTotal = product.Price * quantity;

                await context.OrderItems.AddAsync(new OrderItem
                {
                    OrderItemId = Guid.NewGuid().ToString(),
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    TotalAmount = itemTotal,
                });

                order.TotalAmount += itemTotal;
            }
        }


        //// Message
        //var message = new Message
        //{
        //    MessageId = Guid.NewGuid().ToString(),
        //    SenderId = buyer.UserId,
        //    ReceiverId = seller.UserId,
        //    OrderId = order.OrderId,
        //    Content = "Hello, is the product still available?",
        //};
        //await context.Messages.AddAsync(message);

        // Trade
        //var trade = new Trade
        //{
        //    TradeId = Guid.NewGuid().ToString(),
        //    TradeName = "Sample Trade",
        //    Amount = product.Price,
        //    TradeFromId = buyerWallet.WalletId,
        //    TradeToId = sellerWallet.WalletId,
        //    UserId = buyer.UserId,
        //    TradeIcon = "default",
        //};
        //await context.Trades.AddAsync(trade);

        await context.SaveChangesAsync();
    }

    private static User CreateUser(string username, string email, string roleId) => new()
    {
        UserId = Guid.NewGuid().ToString(),
        Username = username,
        Email = email,
        RoleId = roleId,
        PasswordHash = "123456",
        FullName = username + " name",
        Address = new Address { /* fake address */ },
        ImageUrl = "/images/avatar.png"
    };

    private static Wallet CreateWallet(string userId) => new()
    {
        WalletId = Guid.NewGuid().ToString(),
        UserId = userId,
        AvaibableBalance = 1000000,
        Currency = UnitCurrency.VND
    };
}
