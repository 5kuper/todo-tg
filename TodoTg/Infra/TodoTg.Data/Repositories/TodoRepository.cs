using System.Linq.Expressions;
using TodoTg.Domain.Entities;
using TodoTg.Domain.Repositories;
using Utilities.EFCore;

namespace TodoTg.Data.Repositories
{
    public class TodoRepository(AppDbContext context) : Repository<Todo>(context), ITodoRepository
    {
        protected override Expression<Func<Todo, object>>[] Includes => [(t => t.User)];
    }
}
