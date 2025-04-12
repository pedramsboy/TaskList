using Microsoft.EntityFrameworkCore;
using System;
using TaskList.Data.TaskItems;
using TaskList.Data.TaskLists;
using TaskList.Models.Domain;

namespace TaskList.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        public DbSet<TaskList.Models.Domain.TaskList> TaskLists { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureTaskListModelBuilder();
            modelBuilder.ConfigureTaskItemModelBuilder();
        }
    }
}
