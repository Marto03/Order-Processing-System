using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Repositories;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Ако използвам едно factory, не може да се използват 2-те технологии за комуникация!
//builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
