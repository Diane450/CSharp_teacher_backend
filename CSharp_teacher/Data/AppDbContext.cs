using CSharp_teacher.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CSharp_teacher.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserTaskProgress> UserTaskProgresses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Hint> Hints { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<OpenedHints> OpenedHints { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Submission>().OwnsOne(s => s.Result, b => b.ToJson());

            builder.Entity<Submission>()
           .Property(s => s.Status)
           .HasConversion<string>();

            builder.Entity<UserTaskProgress>().HasKey(ut => new { ut.UserId, ut.TaskId });
            builder.Entity<UserTaskProgress>()
              .HasOne(ut => ut.User)
              .WithMany(u => u.TaskProgresses)
              .HasForeignKey(ut => ut.UserId);

            builder.Entity<UserTaskProgress>()
                   .HasOne(ut => ut.Task)
                   .WithMany(t => t.UserProgresses)
                   .HasForeignKey(ut => ut.TaskId);

            builder.Entity<OpenedHints>().HasKey(oh => new { oh.UserId, oh.HintId });
            builder.Entity<OpenedHints>()
                  .HasOne(oh => oh.User)
                  .WithMany(u => u.OpenedHints)
                  .HasForeignKey(oh => oh.UserId);

            builder.Entity<OpenedHints>()
                   .HasOne(oh => oh.Hint)
                   .WithMany(t => t.OpenedHints)
                   .HasForeignKey(oh => oh.HintId);
        }
    }
}
