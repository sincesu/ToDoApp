using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities.Items;
using ToDo.Domain.Entities.Users;
using ToDo.Domain.Entities.Categories;

namespace ToDo.Persistence
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<ToDoItems> ToDoItems { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<AppUser> AppUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ToDoItems>().HasQueryFilter(x => x.isDeleted == false);
            modelBuilder.Entity<Category>().HasQueryFilter(x => x.isDeleted == false);
            modelBuilder.Entity<AppUser>().HasQueryFilter(x => x.isDeleted == false);

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.name)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(u => u.name)
                .IsUnique();
        }
    }
}
