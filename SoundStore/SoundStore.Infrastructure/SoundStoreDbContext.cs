﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SoundStore.Core.Entities;
using SoundStore.Infrastructure.Configs;
using SoundStore.Infrastructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class SoundStoreDbContext : IdentityDbContext<AppUser>
    {
        public SoundStoreDbContext()
        {
        }

        public SoundStoreDbContext(DbContextOptions<SoundStoreDbContext> options) : base(options)
        {
        }

        public override DbSet<AppUser> Users { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<SubCategory> SubCategories { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<Image> Images { get; set; } = null!;

        public DbSet<Transaction> Transactions { get; set; } = null!;

        public DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            if (env == "Development")
            {
                // https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/simple-logging
                optionsBuilder//.LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName() ?? string.Empty;
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            #region Entity configurations
            new SubCategoryTypeConfig().Configure(builder.Entity<SubCategory>());
            new ProductTypeConfig().Configure(builder.Entity<Product>());
            new OrderDetailTypeConfig().Configure(builder.Entity<OrderDetail>());
            new TransactionTypeConfig().Configure(builder.Entity<Transaction>());
            new OrderTypeConfig().Configure(builder.Entity<Order>());
            new RatingTypeConfiguration().Configure(builder.Entity<Rating>());
            #endregion

            #region Seed data
            builder.SeedUserRoles();
            builder.SeedDataForCategory();
            builder.SeedDataForSubCategory();
            //builder.SeedDataForProduct();
            //builder.SeedDataForImage();
            #endregion
        }
    }
}
