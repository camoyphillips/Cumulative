using System.ComponentModel.DataAnnotations;

namespace Cumulative.Models
{
    public class StudentCourse
    {
        [Key]
        public int studentcoursid { get; set; }

        public int studentid { get; set; }

        public int courseid { get; set; }

        // ✅ Navigation properties
        public required Student student { get; set; }
        public required Course course { get; set; }

        // ✅ Optional constructor for manual object creation
        public StudentCourse() { }

        public StudentCourse(int StudentCourseID, int StudentID, int CourseID)
        {
            studentcoursid = StudentCourseID;
            studentid = StudentID;
            courseid = CourseID;
        }
    }
}
