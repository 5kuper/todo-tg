using Microsoft.EntityFrameworkCore;
using Utilities.Common.Domain;

namespace Utilities.EFCore
{
    public abstract class AppDbContextBase<TChild>(DbContextOptions<TChild> options) : DbContext(options)
        where TChild : DbContext
    {
        public override int SaveChanges()
        {
            SetTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetTimestamps()
        {
            foreach (var entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = DateTime.UtcNow;
                    entry.Entity.Updated = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.Updated = DateTime.UtcNow;
                }
            }
        }
    }
}
