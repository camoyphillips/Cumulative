using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Cumulative.Models
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<TeacherCourse> TeacherCourses { get; set; } // Ensure all tables are mapped

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Server=localhost;Database=school_db;User=root;Password=root;Port=3306;";
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many Relationship: Teacher <-> Course
            modelBuilder.Entity<TeacherCourse>()
                .HasKey(tc => tc.TeacherCourseID);

            modelBuilder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Teacher)
                .WithMany(t => t.TeacherCourses)
                .HasForeignKey(tc => tc.TeacherID);

            modelBuilder.Entity<TeacherCourse>()
                .HasOne(tc => tc.Course)
                .WithMany(c => c.TeacherCourses)
                .HasForeignKey(tc => tc.CourseID);
        }

        // Method to access the MySQL database directly
        public MySqlConnection AccessDatabase()
        {
            return new MySqlConnection(Database.GetDbConnection().ConnectionString);
        }
    }
}
