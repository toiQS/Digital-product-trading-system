using DPTS.Domains;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DPTS.Infrastructures.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Complaint> Complications { get; set; }
        public DbSet<Escrow> Escrows { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet <User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Order)
                .WithMany(o => o.Messages)
                .HasForeignKey(m => m.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

           modelBuilder.Entity<Trade>()
                .HasOne(x => x.TrandeFrom)
                .WithMany(x => x.TradeFroms)
                .HasForeignKey(x => x.TradeFromId)
                .OnDelete(DeleteBehavior.SetNull);
           modelBuilder.Entity<Trade>()
                .HasOne(x => x.TrandeTo)
                .WithMany(x => x.TradeTos)
                .HasForeignKey(x => x.TradeToId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
