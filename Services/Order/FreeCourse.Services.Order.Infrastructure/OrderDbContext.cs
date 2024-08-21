using Microsoft.EntityFrameworkCore; // Entity Framework Core kullanımı için
using System;

namespace FreeCourse.Services.Order.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        // Veritabanında kullanılacak varsayılan şema adı
        public const string DEFAULT_SCHEMA = "ordering";

        // DbContext sınıfının yapıcı metodu. Bağımlılık enjeksiyonu ile DbContextOptions alır.
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        // Veritabanındaki "Orders" tablosunu temsil eden DbSet
        public DbSet<Domain.OrderAggregate.Order> Orders { get; set; }

        // Veritabanındaki "OrderItems" tablosunu temsil eden DbSet
        public DbSet<Domain.OrderAggregate.OrderItem> OrderItems { get; set; }

        // Veritabanı modelinin oluşturulmasını özelleştirmek için kullanılan metot
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // "Orders" tablosunun ismini ve şemasını yapılandırır
            modelBuilder.Entity<Domain.OrderAggregate.Order>().ToTable("Orders", DEFAULT_SCHEMA);

            // "OrderItems" tablosunun ismini ve şemasını yapılandırır
            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().ToTable("OrderItems", DEFAULT_SCHEMA);

            // "OrderItem" tablosundaki "Price" sütununun veri tipini yapılandırır
            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2)");

            // "Order" tablosundaki "Address" değer nesnesini yapılandırır, bu nesne tek başına bir tabloya sahip olmayacak
            modelBuilder.Entity<Domain.OrderAggregate.Order>().OwnsOne(o => o.Address).WithOwner();

            // Taban sınıfın OnModelCreating metodu çağrılır
            base.OnModelCreating(modelBuilder);
        }
    }
}
