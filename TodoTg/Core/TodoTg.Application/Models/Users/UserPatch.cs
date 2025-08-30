using TodoTg.Domain.Entities;
using TodoTg.Domain.ValueObj;
using Utilities.Application;

namespace TodoTg.Application.Models.Users
{
    public class UserPatch : IPatch<User>
    {
        public string? Name { get; set; }

        public AppLanguage? Language { get; set; }

        public void Apply(User target)
        {
            if (Name != null)
                target.Name = Name;

            if (Language != null)
                target.Language = Language.Value;
        }
    }
}
