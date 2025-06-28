using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Data.Source;

public class Dsw2025TpiContext: DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options) : base(options)
    {
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
            p.Property(q => q.Id).IsRequired().HasColumnName("id");
            p.ToTable("Products");
            modelBuilder.Entity<Product>().HasKey(q => q.Id);
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
            p.Property(q => q.Id).IsRequired().HasColumnName("id");
            p.ToTable("Orders");
            modelBuilder.Entity<Order>().HasKey(q => q.Id);
        });

        modelBuilder.Entity<OrderItem>(p =>
        {
            p.Property(q => q.skuProduct).HasMaxLength(30);
            p.Property(q => q.quantity).HasMaxLength(30).IsRequired();
            p.Property(q => q.subTotal).HasMaxLength(30);
            p.Property(q => q.orderId).HasMaxLength(30);
            p.Property(q => q.Id).IsRequired().HasColumnName("id");
            p.ToTable("OrderItems");
            modelBuilder.Entity<Order>().HasKey(q => q.Id);
        });

        modelBuilder.Entity<Customer>(p =>
        {
            p.Property(q => q.eMail).HasMaxLength(30);
            p.Property(q => q.name).HasMaxLength(30);
            p.Property(q => q.phoneNumber).HasMaxLength(20);
            p.Property(q => q.Id).IsRequired().HasColumnName("id");
            p.ToTable("Customers");
            modelBuilder.Entity<Customer>().HasKey(q => q.Id);
        });
    }
}
