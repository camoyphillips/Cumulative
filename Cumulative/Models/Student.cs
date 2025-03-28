using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cumulative.Models
{
    public class Student
    {
        [Key] // Primary Key
        public int StudentID { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; }

        public int StudentNumber { get; set; }

        // Simplified collection initialization
        public ICollection<Course> Courses { get; set; } = [];
    }
}
