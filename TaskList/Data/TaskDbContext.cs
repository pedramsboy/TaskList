using Microsoft.EntityFrameworkCore;
using System;
using TaskList.Data.EntityConfigs;
using TaskList.Domain.Entity;

namespace TaskList.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        public DbSet<TaskList.Domain.Entity.TaskList> TaskLists { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureTaskListModelBuilder();
            modelBuilder.ConfigureTaskItemModelBuilder();
        }
    }
}
