using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Data
{
    public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        /// <summary>
        /// Това позволява на EF Core по време на design-time (Add-Migration) да създаде контекста ръчно, без да се нуждае от DI контейнера и без да стартира Hosted Services.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NotificationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new NotificationDbContext(optionsBuilder.Options);
        }
    }
}
