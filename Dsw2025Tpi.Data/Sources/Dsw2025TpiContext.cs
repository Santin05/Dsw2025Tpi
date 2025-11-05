using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Domain.Entities;
using System.Text.Json;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Dsw2025Tpi.Data.Source;

public class Dsw2025TpiContext : IdentityDbContext<IdentityUser>
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options) : base(options)
    {
    }

    public void LoadData(Dsw2025TpiContext context)
    {
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Customers");

<<<<<<< HEAD
        var fileName = @"C:\Users\Santino\Desktop\TpiCarpeta\Dsw2025Tpi\Dsw2025Tpi.Data\Sources\customers.json";
=======
        var fileName = @"C:\Users\Santino\Desktop\PRIMERA PRESENTACIÓN (errores coregidos)\Dsw2025Tpi\Dsw2025Tpi.Data\Sources\customers.json";
>>>>>>> 8473edca8a8869e61a9cdf3446a7137c3faf062e
        var read = File.ReadAllText(fileName);
        var data = JsonSerializer.Deserialize<List<Customer>>(read);
        if (data != null)
        {
            foreach (var c in data)
            {
                context.Add(c);
            }
        }
        context.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(p =>
        {
            p.Property(q => q.sku).HasMaxLength(30);
            p.Property(q => q.name).HasMaxLength(50);
            p.Property(q => q.description).HasMaxLength(80);
            p.Property(q => q.internalCode).HasMaxLength(30);
            p.Property(q => q.currentUnitPrice).HasMaxLength(30);
            p.Property(q => q.stockQuantity).HasMaxLength(30).IsRequired();
            p.Property(q => q.id).IsRequired().HasColumnName("id");
            p.ToTable("Products");
            modelBuilder.Entity<Product>().HasKey(q => q.id);
        });

        modelBuilder.Entity<Order>().Ignore(q => q.orderItems);

        modelBuilder.Entity<Order>(p =>
        {
            p.Property(q => q.date).HasMaxLength(30);
            p.Property(q => q.shippingAddress).HasMaxLength(30);
            p.Property(q => q.notes).HasMaxLength(30);
            p.Property(q => q.totalAmount).HasMaxLength(30).IsRequired();
            p.Property(q => q.status).HasMaxLength(10);
            p.Property(q => q.customerId).IsRequired();
            p.Property(q => q.id).IsRequired().HasColumnName("id");
            p.ToTable("Orders");
            modelBuilder.Entity<Order>().HasKey(q => q.id);
        });

        modelBuilder.Entity<OrderItem>(p =>
        {
            p.Property(q => q.skuProduct).HasMaxLength(30);
            p.Property(q => q.quantity).HasMaxLength(30).IsRequired();
            p.Property(q => q.subTotal).HasMaxLength(30);
            p.Property(q => q.orderId).HasMaxLength(30);
            p.Property(q => q.id).IsRequired().HasColumnName("id");
            p.ToTable("OrderItems");
            modelBuilder.Entity<Order>().HasKey(q => q.id);
        });

        modelBuilder.Entity<Customer>(p =>
        {
            p.Property(q => q.eMail).HasMaxLength(30);
            p.Property(q => q.name).HasMaxLength(30);
            p.Property(q => q.phoneNumber).HasMaxLength(20);
            p.Property(q => q.id).IsRequired().HasColumnName("id");
            p.ToTable("Customers");
            modelBuilder.Entity<Customer>().HasKey(q => q.id);
        });
    }
}
