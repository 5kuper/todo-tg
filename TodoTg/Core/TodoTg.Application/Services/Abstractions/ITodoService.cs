using TodoTg.Application.Models.Todos;
using Utilities.Application;
using Utilities.Common;

namespace TodoTg.Application.Services.Abstractions
{
    public interface ITodoService : ICrudServiceBase<TodoDto, TodoDto, TodoPatch>
    {
        Task<IList<TodoDto>> ListAsync(Guid userId);

        Task<PagedResult<TodoDto>> ListAsync(Guid userId, int page, int size = 10);
    }
}
