using Elasticsearch.Net;
using FluentValidation.AspNetCore;
using Infrastructure.Extensions;
using Infrastructure.Messaging.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nest;
using OrderService;
using OrderService.Data;
using OrderService.Middleware;
using OrderService.Repositories;
using OrderService.Services;
using OrderService.Validators;
using Serilog;
using Shared.Messaging;
using System.Text;


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.DurableHttpUsingFileSizeRolledBuffers(
        requestUri: "http://localhost:31311", // Logstash HTTP Input порт
        bufferBaseFileName: "Logs/logbuffer"
    )
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// �������� OrderDbContext � �� ������� �� �������� PostgreSQL
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. ����������� �� �������� � DI ����������
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrdersService, OrdersService>();

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateOrderDtoValidator>();
    });

// Зареждаме настройките
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
});

var messagingType = builder.Configuration["Messaging:Type"]; // "Kafka" или "RabbitMQ"

if (messagingType == "Kafka")
{
    builder.Services.AddSingleton<IMessageBusPublisher, KafkaService>();
}
else if (messagingType == "RabbitMQ")
{
    builder.Services.AddSingleton<IMessageBusPublisher, RabbitMQService>();
}
else
{
    throw new Exception("Invalid messaging type configured");
}
// �������� ���������� (��� ��������� API-��)

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<LogService>();
//builder.Services.AddSingleton<IMessageBusPublisher, RabbitMQService>();
    builder.Services.AddInfrastructure(); // добавя Publisher-a

var app = builder.Build();

// Middleware �� ���������� � Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseAuthentication(); // Много важно
app.UseAuthorization();

app.MapControllers();

app.Run();
