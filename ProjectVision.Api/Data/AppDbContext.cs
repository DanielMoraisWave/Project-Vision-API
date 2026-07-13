using Microsoft.EntityFrameworkCore;
using ProjectVision.Api.Models;

namespace ProjectVision.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<int>("ProductIdSequence")
            .StartsAt(100003)
            .IncrementsBy(1)
            .HasMin(100000)
            .HasMax(999999)
            .IsCyclic(false);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);

            entity.Property(product => product.Id)
                .HasDefaultValueSql(
                    "NEXT VALUE FOR ProductIdSequence");

            entity.Property(product => product.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(product => product.Description)
                .HasMaxLength(500);

            entity.Property(product => product.Price)
                .HasPrecision(18, 2);

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Products_Price",
                    "[Price] > 0");

                table.HasCheckConstraint(
                    "CK_Products_Stock",
                    "[Stock] >= 0");
            });

            entity.HasData(
                new Product
                {
                    Id = 100000,
                    Name = "Laptop",
                    Description = "Example laptop",
                    Price = 999.99m,
                    Stock = 10
                },
                new Product
                {
                    Id = 100001,
                    Name = "Wireless Mouse",
                    Description = "Example wireless mouse",
                    Price = 29.99m,
                    Stock = 30
                },
                new Product
                {
                    Id = 100002,
                    Name = "Mechanical Keyboard",
                    Description = "Example keyboard",
                    Price = 89.99m,
                    Stock = 15
                });
        });
    }
}