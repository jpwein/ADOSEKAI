using Microsoft.EntityFrameworkCore;
using SekaiADO24.Models;

namespace SekaiADO24.Data;

public sealed class CollegeDbContext : DbContext
{
    public CollegeDbContext(DbContextOptions<CollegeDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().ToTable("Students");
        modelBuilder.Entity<Group>().ToTable("Groups");
        modelBuilder.Entity<Subject>().ToTable("Subjects");
        modelBuilder.Entity<Teacher>().ToTable("Teachers");
        modelBuilder.Entity<Grade>().ToTable("Grades");
        modelBuilder.Entity<Grade>().Property(grade => grade.Value).HasPrecision(5, 2);

        modelBuilder.Entity<Teacher>()
            .HasOne(teacher => teacher.Subject)
            .WithMany()
            .HasForeignKey(teacher => teacher.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Student>()
            .HasOne(student => student.Group)
            .WithMany(group => group.Students)
            .HasForeignKey(student => student.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Grade>()
            .HasOne(grade => grade.Student)
            .WithMany(student => student.Grades)
            .HasForeignKey(grade => grade.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Grade>()
            .HasOne(grade => grade.Subject)
            .WithMany(subject => subject.Grades)
            .HasForeignKey(grade => grade.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Grade>()
            .HasOne(grade => grade.Teacher)
            .WithMany(teacher => teacher.Grades)
            .HasForeignKey(grade => grade.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
