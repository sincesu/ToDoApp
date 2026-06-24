using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities.Items;
using ToDo.Domain.Entities.Users;
using ToDo.Domain.Entities.Categories;
using ToDo.Domain.Entities.Comments;
using ToDo.Domain.Entities.Histories;
using Todo.Domain.Entities;

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

        public DbSet<Comment> Comment { get; set; }

        public DbSet<TaskHistory> TaskHistory { get; set; }

        public DbSet<FileAttachment> Attachment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Global Query Filters

            modelBuilder.Entity<ToDoItems>().HasQueryFilter(x => x.isDeleted == false);
            modelBuilder.Entity<Category>().HasQueryFilter(x => x.isDeleted == false);
            modelBuilder.Entity<AppUser>().HasQueryFilter(x => x.isDeleted == false);
            modelBuilder.Entity<TaskHistory>().HasQueryFilter(x => !x.isDeleted);

            modelBuilder.Entity<FileAttachment>()
            .HasQueryFilter(x =>
                !x.isDeleted
                && (x.ToDoItem == null || !x.ToDoItem.isDeleted)
                && (x.Comment == null || !x.Comment.isDeleted)
            );

            modelBuilder.Entity<Comment>()
           .HasQueryFilter(x =>
               !x.isDeleted
               && !x.AppUser.isDeleted
               && !x.ToDoItems.isDeleted
           );

            //Multiple Cascade Paths

            modelBuilder.Entity<FileAttachment>()
                .HasOne(a => a.ToDoItem)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.ToDoItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FileAttachment>()
                .HasOne(a => a.Comment)
                .WithMany(c => c.Attachments)
                .HasForeignKey(a => a.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
            .HasOne(c => c.ToDoItems)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.ToDoItemsId)
            .OnDelete(DeleteBehavior.NoAction);

            //Index Configuration

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.name)
                .IsUnique()
                .HasFilter("isDeleted = 0");

            modelBuilder.Entity<Category>()
                .HasIndex(u => u.name)
                .IsUnique()
                .HasFilter("isDeleted = 0");
        }
    }
}
