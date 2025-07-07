using DPTS.Domains;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        #region User Domain
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserSecurity> UserSecurities { get; set; }
        public DbSet<Role> Roles { get; set; }
        #endregion

        #region Store & Communication
        public DbSet<Store> Stores { get; set; }
        public DbSet<Message> Messages { get; set; }
        #endregion

        #region Product Domain
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<AdjustmentRule> AdjustmentRules { get; set; }
        public DbSet<ProductAdjustment> ProductAdjustments { get; set; }
        #endregion

        #region Order Domain
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderPayment> OrderPayments { get; set; }
        #endregion

        #region Wallet & Payment
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Escrow> Escrows { get; set; }
        #endregion

        #region Complaint
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintImage> ComplaintImages { get; set; }
        #endregion

        #region Miscellaneous
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Log> Logs { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Embedded Address in UserProfile
            modelBuilder.Entity<UserProfile>()
                .OwnsOne(u => u.Address, a =>
                {
                    a.Property(x => x.Street).HasColumnName("street");
                    a.Property(x => x.District).HasColumnName("district");
                    a.Property(x => x.City).HasColumnName("city");
                    a.Property(x => x.Country).HasColumnName("country");
                    a.Property(x => x.PostalCode).HasColumnName("postal_code");
                });

            // Seeding default roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = "Admin", RoleName = "Admin", Description = "" },
                new Role { RoleId = "Buyer", RoleName = "Buyer", Description = "" },
                new Role { RoleId = "Seller", RoleName = "Seller", Description = "" });
        }
    }
}
