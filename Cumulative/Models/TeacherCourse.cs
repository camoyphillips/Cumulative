using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cumulative.Models
{
    public class TeacherCourse(int teacherCourseID, int teacherID, int courseID)
    {
        [Key]
        public int TeacherCourseID { get; set; } = teacherCourseID;

        public int TeacherID { get; set; } = teacherID;
        public required Teacher Teacher { get; set; }

        public int CourseID { get; set; } = courseID;
        public required Course Course { get; set; }
    }
}
