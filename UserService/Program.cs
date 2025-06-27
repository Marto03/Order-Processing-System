using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using UserService.Data;
using UserService.Mapping;
using UserService.Repositories;
using UserService.Services;
using UserService.Validators;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DB Context (PostgreSQL)
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add AutoMapper
builder.Services.AddAutoMapper(typeof(UserProfile));
    //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 3. Add FluentValidation
builder.Services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
builder.Services.AddScoped<IValidator<UserDTO>, UserDTOValidator>();

// 4. Add Repository and Service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

// 5. Add Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000") // 5173 -> using Vite / 3000
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();


app.UseCors(MyAllowSpecificOrigins);
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
