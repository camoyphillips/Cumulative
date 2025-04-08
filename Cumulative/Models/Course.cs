using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cumulative.Models
{
    public class Course
    {
        [Key]
        public int courseid { get; set; }

        [Required]
        public string? coursecode { get; set; }

        [Required]
        public string? coursename { get; set; }

        public int teacherid { get; set; }

        public DateTime startdate { get; set; }

        public DateTime finishdate { get; set; }

        // ✅ Navigation property for many-to-many
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        public Course() { }

        public Course(int CourseID, string CourseCode, string CourseName, int TeacherID, DateTime StartDate, DateTime FinishDate)
        {
            courseid = CourseID;
            coursecode = CourseCode;
            coursename = CourseName;
            teacherid = TeacherID;
            startdate = StartDate;
            finishdate = FinishDate;
        }
    }
}
