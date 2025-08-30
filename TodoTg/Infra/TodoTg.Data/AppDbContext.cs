using Microsoft.EntityFrameworkCore;
using TodoTg.Domain.Entities;
using Utilities.EFCore;

namespace TodoTg.Data
{
    public class AppDbContext : AppDbContextBase<AppDbContext>
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Todo> Todos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Todos)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}
