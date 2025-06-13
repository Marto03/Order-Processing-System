using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Models;
using UserService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using UserService.Data;
using UserService.DTOs;
using UserService.Models;
using FluentValidation;
using FluentValidation.Results;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<UserCreateDto> _validator;

        public UsersController(UserDbContext context, IMapper mapper, IValidator<UserCreateDto> validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        // Регистрация на потребител
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            // Валидираме входните данни
            ValidationResult result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            // Преобразуваме DTO към Entity
            var user = _mapper.Map<User>(dto);

            // Добавяме потребителя в базата данни
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // Извличане на потребител по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}
