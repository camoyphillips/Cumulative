using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cumulative.Models
{
    public class Student
    {
        [Key]
        public int studentid { get; set; }

        [Required]
        public string? studentfname { get; set; }

        [Required]
        public string? studentlname { get; set; }

        public DateTime enroldate { get; set; }

        public string studentnumber { get; set; }

        // ✅ Navigation property for many-to-many
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

        public Student() { }

        public Student(int StudentID, string FirstName, string LastName, DateTime EnrolDate, string StudentNumber)
        {
            studentid = StudentID;
            studentfname = FirstName;
            studentlname = LastName;
            enroldate = EnrolDate;
            studentnumber = StudentNumber;
        }
    }
}
