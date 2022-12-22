using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SchoolDbProject.Models;

namespace SchoolDbProject.Data
{
    public partial class SchoolDbContext : DbContext
    {
        public SchoolDbContext()
        {
        }

        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Grade> Grades { get; set; } = null!;
        public virtual DbSet<RelationShip> RelationShips { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Title> Titles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=ALDOR007; Initial Catalog = SchoolDb; Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Class");

                entity.Property(e => e.ClassName).HasMaxLength(30);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course");

                entity.Property(e => e.CourseName).HasMaxLength(50);

                entity.Property(e => e.FkEmployeeId).HasColumnName("FK_EmployeeId");

                entity.HasOne(d => d.FkEmployee)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.FkEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Course__FK_Emplo__34C8D9D1");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.FkTitleId).HasColumnName("FK_TitleId");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.HasOne(d => d.FkTitle)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.FkTitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employee__FK_Tit__32E0915F");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.ToTable("Grade");

                entity.Property(e => e.GradeLevel)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<RelationShip>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RelationShip");

                entity.Property(e => e.FkCourseId).HasColumnName("FK_CourseId");

                entity.Property(e => e.FkGradeId).HasColumnName("FK_GradeId");

                entity.Property(e => e.FkGradedByTeacherId).HasColumnName("FK_GradedByTeacherId");

                entity.Property(e => e.FkStudentId).HasColumnName("FK_StudentId");

                entity.Property(e => e.SetDate).HasColumnType("date");

                entity.HasOne(d => d.FkCourse)
                    .WithMany()
                    .HasForeignKey(d => d.FkCourseId)
                    .HasConstraintName("FK__RelationS__FK_Co__300424B4");

                entity.HasOne(d => d.FkGrade)
                    .WithMany()
                    .HasForeignKey(d => d.FkGradeId)
                    .HasConstraintName("FK__RelationS__FK_Gr__30F848ED");

                entity.HasOne(d => d.FkGradedByTeacher)
                    .WithMany()
                    .HasForeignKey(d => d.FkGradedByTeacherId)
                    .HasConstraintName("FK__RelationS__FK_Gr__31EC6D26");

                entity.HasOne(d => d.FkStudent)
                    .WithMany()
                    .HasForeignKey(d => d.FkStudentId)
                    .HasConstraintName("FK__RelationS__FK_St__2F10007B");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.FkClassId).HasColumnName("FK_ClassId");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.SsNumber)
                    .HasMaxLength(15)
                    .HasColumnName("SS_Number")
                    .IsFixedLength();

                entity.HasOne(d => d.FkClass)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.FkClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Student__FK_Clas__33D4B598");
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.ToTable("Title");

                entity.Property(e => e.TitleName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
