using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using UserService.DTOs;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _repository;
        private readonly IValidator<UserCreateDto> _validator;
        private readonly IMapper _mapper;

        public UsersService(IUserRepository repository, IValidator<UserCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<User?> CreateUserAsync(UserCreateDto dto)
        {
            // Валидираме DTO-то
            ValidationResult result = _validator.Validate(dto);

            if (!result.IsValid)
            {
                // За реална система: може да върнем обект с грешки
                throw new ValidationException(result.Errors);
            }

            // Мапваме от DTO към Entity
            var user = _mapper.Map<User>(dto);

            // Запазваме в базата
            await _repository.CreateAsync(user);

            return user;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
