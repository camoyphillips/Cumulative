// Cumulative/Models/Teacher.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cumulative.Models
{
    public class Teacher
    {
        [Key]
        public int teacherid { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string? teacherfname { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string? teacherlname { get; set; }

        [Required(ErrorMessage = "Employee number is required.")]
        // add a Regex pattern 
        // [RegularExpression(@"^EMP\d{3}$", ErrorMessage = "Employee number must be in format EMP###.")]
        public string? employeenumber { get; set; }

        public DateTime? hiredate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary cannot be negative.")]
        public decimal salary { get; set; }

        [MaxLength(255, ErrorMessage = "Work phone number is too long (max 255 characters).")]
        public string? teacherworkphone { get; set; }

        // Navigation property for many-to-many relationships 
        public ICollection<StudentCourse> TeacherCourses { get; set; } = new List<StudentCourse>();

        public Teacher() { }

        public Teacher(int TeacherID, string FirstName, string LastName, string EmployeeNumber, DateTime? HireDate, decimal Salary, string? WorkPhone)
        {
            teacherid = TeacherID;
            teacherfname = FirstName;
            teacherlname = LastName;
            employeenumber = EmployeeNumber;
            hiredate = HireDate;
            salary = Salary;
            teacherworkphone = WorkPhone;
        }
    }
}
