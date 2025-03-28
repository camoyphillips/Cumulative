using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cumulative.Models
{
    public class Course(int courseID, string courseCode, string courseName, int teacherID, DateTime startDate, DateTime finishDate)
    {
        [Key]
        public int CourseID { get; set; } = courseID;

        [Required]
        public string CourseCode { get; set; } = courseCode;

        [Required]
        public string CourseName { get; set; } = courseName;

        public int TeacherID { get; set; } = teacherID;

        public DateTime StartDate { get; set; } = startDate;

        public DateTime FinishDate { get; set; } = finishDate;

        // Many-to-Many Relationship with Teachers
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
    }
}
