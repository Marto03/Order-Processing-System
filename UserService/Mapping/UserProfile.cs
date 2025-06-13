using AutoMapper;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Mapping
{
    // Профил за AutoMapper – свързва DTO с Entity
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDto, User>();
        }
    }
}
