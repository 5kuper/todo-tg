using System.ComponentModel.DataAnnotations;

namespace Utilities.Common.Domain
{
    public class Entity
    {
        [Key] public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}