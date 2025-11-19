using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks => Set<TaskItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<TaskItem>( entity =>
        //    {
        //        entity.ToTable("Tasks");
        //        entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
        //        entity.Property(t => t.IsCompleted).IsRequired().HasDefaultValue(false);
        //        entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");
        //    });
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("Tasks");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(t => t.IsCompleted)
                      .HasDefaultValue(false);

                entity.Property(t => t.Priority)
                      .HasDefaultValue(0);

                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
