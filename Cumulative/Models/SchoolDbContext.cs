using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Cumulative.Models
{
    public class SchoolDbContext : DbContext
    {
        private const string ConnectionString = "Server=localhost;Database=school_db;User=root;Password=root;Port=3306;";

        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> TeacherCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many Relationship: Teacher <-> Course
            modelBuilder.Entity<StudentCourse>()
                .HasKey(tc => tc.studentcoursid);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(tc => tc.student)
                .WithMany(t => t.StudentCourses)
                .HasForeignKey(nameof(StudentCourse.studentid));

            modelBuilder.Entity<StudentCourse>()
                .HasOne(tc => tc.course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(nameof(StudentCourse.courseid));
        }

        // Custom method to directly access the MySQL database
        public MySqlConnection AccessDatabase()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
