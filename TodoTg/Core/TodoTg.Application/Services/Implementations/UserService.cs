using AutoMapper;
using TodoTg.Application.Models.Users;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Domain.Entities;
using TodoTg.Domain.Repositories;
using Utilities.Application;

namespace TodoTg.Application.Services.Implementations
{
    public class UserService(IUserRepository repo, IMapper mapper)
        : CrudServiceBase<User, IUserRepository, UserDto, UserInput, UserPatch>(repo, mapper), IUserService
    {
        public async Task<Guid> EnsureCreatedForTelegram(UserInput input)
        {
            if (input.TgChatId == null)
                throw new ArgumentException("TgChatId cannot be null.", nameof(input));

            var user = await Repository.GetAsync(u => u.TgChatId == input.TgChatId);
            user ??= await CreateUnmappedAsync(input);

            return user.Id;
        }
    }
}
