using TodoTg.Application.Models.Todos;
using TodoTg.Application.Models.Users;
using TodoTg.Domain.Entities;

namespace TodoTg
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserInput, User>();

            CreateMap<Todo, TodoDto>();
            CreateMap<TodoDto, Todo>();
        }
    }
}
