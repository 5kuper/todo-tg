using TodoTg.Domain.Entities;
using Utilities.Common.Domain;

namespace TodoTg.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>;

    public interface ITodoRepository : IRepository<Todo>;
}
