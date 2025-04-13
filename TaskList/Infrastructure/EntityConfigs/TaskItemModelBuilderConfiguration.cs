using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;
using TaskList.Domain.Entity;

namespace TaskList.Data.EntityConfigs
{
    public static class TaskItemModelBuilderConfiguration
    {
        public static void ConfigureTaskItemModelBuilder(this ModelBuilder builder)
        {

            builder.Entity<TaskItem>().HasKey(t => t.Id);

            // Query Filter for soft delete
            builder.Entity<TaskItem>().HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<TaskItem>()
                .HasOne(t => t.TaskList)
                .WithMany(l => l.Tasks)
                .HasForeignKey(t => t.TaskListId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskItem>()
            .Property(t => t.Title)
            .IsUnicode(true)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(255);

            builder.Entity<TaskItem>()
                .Property(t => t.Description)
                .HasColumnType("Nvarchar(500)")
                .IsRequired()
                .IsUnicode();
        }
    }
}
