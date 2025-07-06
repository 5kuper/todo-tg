using AutoMapper;
using TodoTg.Application.Models.Todos;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Domain.Entities;
using TodoTg.Domain.Repositories;
using Utilities.Application;
using Utilities.Common;

namespace TodoTg.Application.Services.Implementations
{
    public class TodoService(ITodoRepository repo, IMapper mapper)
        : CrudServiceBase<Todo, ITodoRepository, TodoDto, TodoDto, TodoPatch>(repo, mapper), ITodoService
    {
        public async Task<IList<TodoDto>> ListAsync(Guid userId)
        {
            var entities = await Repository.ListAsync(t => t.UserId == userId);
            return entities.Select(Mapper.Map<TodoDto>).ToList();
        }

        public async Task<PagedResult<TodoDto>> ListAsync(Guid userId, int page, int size = 10)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Page cannot be less than one.");

            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Size cannot be less than one.");

            var totalCount = await Repository.CountAsync();
            var entities = await Repository.ListAsync(t => t.UserId == userId, (page - 1) * size, size);

            return new PagedResult<TodoDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(Mapper.Map<TodoDto>).ToList(),
                CurrentPage = page,
                PageSize = size,
                NumPages = (int)Math.Ceiling((double)totalCount / size)
            };
        }
    }
}
