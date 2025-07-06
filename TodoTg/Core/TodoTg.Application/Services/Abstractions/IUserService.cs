using TodoTg.Application.Models.Users;
using Utilities.Application;

namespace TodoTg.Application.Services.Abstractions
{
    public interface IUserService : ICrudServiceBase<UserDto, UserInput, UserPatch>
    {
        Task<Guid> EnsureCreatedForTelegram(UserInput input);
    }
}
