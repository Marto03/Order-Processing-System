using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Middleware;
using OrderService.Repositories;
using OrderService.Services;
using OrderService.Validators;

var builder = WebApplication.CreateBuilder(args);

// Добавяме OrderDbContext и му казваме да използва PostgreSQL
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрация на слоевете в DI контейнера
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrdersService, OrdersService>();

builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateOrderDtoValidator>();
    });

// Добавяме контролери (или минимални API-та)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Middleware за разработка – Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ValidationExceptionMiddleware>();

app.Run();
