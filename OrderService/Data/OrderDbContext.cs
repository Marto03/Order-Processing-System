using OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Data
{
    // Контекстът отговаря за връзката между приложението и базата данни
    public class OrderDbContext : DbContext
    {
        // Конструктор, който приема DbContextOptions и го предава нагоре
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        // Таблици в базата данни
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка за връзката "един към много" между Order и OrderItems
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order!)
                .HasForeignKey(i => i.OrderId);
        }
    }
}
