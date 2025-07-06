using System.Linq.Expressions;
using TodoTg.Domain.Entities;
using TodoTg.Domain.Repositories;
using Utilities.EFCore;

namespace TodoTg.Data.Repositories
{
    public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
    {
        protected override Expression<Func<User, object>>[] Includes => [(u => u.Todos)];
    }
}
