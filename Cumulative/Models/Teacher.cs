using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cumulative.Models
{
    public class Teacher(int teacherID, string firstName, string lastName, string employeeNumber, DateTime? hireDate, decimal salary)
    {
        [Key]
        public int TeacherID { get; set; } = teacherID;

        [Required]
        public string FirstName { get; set; } = firstName;

        [Required]
        public string LastName { get; set; } = lastName;

        [Required]
        public string EmployeeNumber { get; set; } = employeeNumber;

        public DateTime? HireDate { get; set; } = hireDate;

        public decimal Salary { get; set; } = salary;

        // Many-to-Many Relationship with Courses
        public ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
    }
}
