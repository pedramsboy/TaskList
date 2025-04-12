using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskList.Models.Domain;

namespace TaskList.Data.TaskLists
{
    public static class TaskListModelBuilderConfiguration
    {
        public static void ConfigureTaskListModelBuilder(this ModelBuilder builder)
        {
            builder.Entity<TaskList.Models.Domain.TaskList>().HasKey(tl => tl.Id);

            // Query Filter for soft delete
            builder.Entity<TaskList.Models.Domain.TaskList>().HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<TaskList.Models.Domain.TaskList>()
            .Property(tl => tl.Name)
            .HasColumnType("Nvarchar(100)")
            .IsRequired()
            .IsUnicode();
        }

    }
}
