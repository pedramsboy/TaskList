using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskList.Data.EntityConfigs
{
    public static class TaskListModelBuilderConfiguration
    {
        public static void ConfigureTaskListModelBuilder(this ModelBuilder builder)
        {
            builder.Entity<TaskList.Domain.Entity.TaskList>().HasKey(tl => tl.Id);

            // Query Filter for soft delete
            builder.Entity<TaskList.Domain.Entity.TaskList>().HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<TaskList.Domain.Entity.TaskList>()
            .Property(tl => tl.Name)
            .HasColumnType("Nvarchar(100)")
            .IsRequired()
            .IsUnicode();
        }

    }
}
