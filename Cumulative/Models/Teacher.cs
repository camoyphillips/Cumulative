using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cumulative.Models
{
    public class Teacher
    {
        [Key]
        public int teacherid { get; set; }

        [Required]
        public string? teacherfname { get; set; }

        [Required]
        public string? teacherlname { get; set; }

        [Required]
        public string? employeenumber { get; set; }

        public DateTime? hiredate { get; set; }

        public decimal salary { get; set; }

        public string? teacherworkphone { get; set; }

        public ICollection<StudentCourse> TeacherCourses { get; set; } = new List<StudentCourse>();

        public Teacher() { }

        public Teacher(int TeacherID, string FirstName, string LastName, string EmployeeNumber, DateTime? HireDate, decimal Salary)
        {
            teacherid = TeacherID;
            teacherfname = FirstName;
            teacherlname = LastName;
            employeenumber = EmployeeNumber;
            hiredate = HireDate;
            salary = Salary;
            teacherworkphone = teacherworkphone;
    }
    }
}
