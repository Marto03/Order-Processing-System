using UserService.Data;
using UserService.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using UserService.DTOs;
using UserService.Services;
using UserService.Validators;
using AutoMapper;
using UserService.Mapping;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DB Context (PostgreSQL)
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserConnection")));

// 2. Add AutoMapper
builder.Services.AddAutoMapper(typeof(UserProfile));

// 3. Add FluentValidation
builder.Services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();

// 4. Add Repository and Service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

// 5. Add Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
