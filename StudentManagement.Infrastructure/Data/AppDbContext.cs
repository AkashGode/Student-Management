using Microsoft.EntityFrameworkCore;
using StudentManagement.Core.Entities;

namespace StudentManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(s => s.Email).IsUnique();
            entity.Property(s => s.Course).IsRequired().HasMaxLength(150);
            entity.Property(s => s.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
        });

        // Seed data for demo
        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, Name = "Akash Sharma", Email = "akash@example.com", Age = 24, Course = "Computer Science", CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Student { Id = 2, Name = "Priya Patel", Email = "priya@example.com", Age = 22, Course = "Data Science", CreatedDate = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Student { Id = 3, Name = "Rahul Gupta", Email = "rahul@example.com", Age = 23, Course = "Machine Learning", CreatedDate = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
