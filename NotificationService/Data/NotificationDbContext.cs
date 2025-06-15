using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace NotificationService.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<KafkaMessage> KafkaMessages { get; set; }
        public DbSet<RabbitMqMessage> RabbitMqMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
