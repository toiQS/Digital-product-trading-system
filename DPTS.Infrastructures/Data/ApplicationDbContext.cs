﻿using DPTS.Domains;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DPTS.Infrastructures.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintImage> ComplaintImages { get; set; }
        public DbSet<Escrow> Escrows { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
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
        public DbSet<Store> Stores { get; set; }
        public DbSet<Tax> Taxs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .OwnsOne(u => u.Address, a =>
            {
                a.Property(x => x.Street).HasColumnName("street");
                a.Property(x => x.District).HasColumnName("district");
                a.Property(x => x.City).HasColumnName("city");
                a.Property(x => x.Country).HasColumnName("country");
                a.Property(x => x.PostalCode).HasColumnName("postal_code");
            });

            modelBuilder.Entity<Message>()
               .HasOne(m => m.SenderUser)
               .WithMany(u => u.SentMessages)
               .HasForeignKey(m => m.SenderUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReceiverUser)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.SenderStore)
                .WithMany() // Không ánh xạ ngược về Store
                .HasForeignKey(m => m.SenderStoreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ReceiverStore)
                .WithMany() // Không ánh xạ ngược về Store
                .HasForeignKey(m => m.ReceiverStoreId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Trade>()
                .HasOne(x => x.TradeFrom)
                .WithMany(x => x.TradeFroms)
                .HasForeignKey(x => x.TradeFromId)
                .OnDelete(DeleteBehavior.SetNull);
           modelBuilder.Entity<Trade>()
                .HasOne(x => x.TradeTo)
                .WithMany(x => x.TradeTos)
                .HasForeignKey(x => x.TradeToId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = "Admin", RoleName = "Admin", Description = "" },
                new Role { RoleId = "Buyer", RoleName = "Buyer", Description = "" },
                new Role { RoleId = "Seller", RoleName = "Seller", Description = "" });

        }
    }
}
