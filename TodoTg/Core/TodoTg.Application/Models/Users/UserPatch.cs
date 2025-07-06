using TodoTg.Domain.Entities;
using Utilities.Application;

namespace TodoTg.Application.Models.Users
{
    public class UserPatch : IPatch<User>
    {
        public string? Name { get; set; }

        public void Apply(User target)
        {
            if (Name != null)
                target.Name = Name;
        }
    }
}
