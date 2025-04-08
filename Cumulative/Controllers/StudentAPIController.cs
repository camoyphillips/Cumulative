using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System;
using System.Collections.Generic;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;

namespace Cumulative.Controllers
{
    [Route("StudentAPI")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StudentAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Students in the system
        /// </summary>
        /// <example>
        /// GET api/Student/ListStudents -> [{"StudentID":1,"FirstName":"John", "LastName":"Doe", "EnrollmentDate":"2023-01-15", "StudentNumber":"1001"},...]
        /// </example>
        /// <returns>
        /// A list of student objects
        /// </returns>
        [HttpGet]
        [Route("ListStudents")]
        public List<Student> ListStudents()
        {
            List<Student> Students = new List<Student>();

            using (MySqlConnection Connection = (MySqlConnection)_context.Database.GetDbConnection())
            {
                Connection.Open();
                using (MySqlCommand Command = Connection.CreateCommand())
                {
                    Command.CommandText = "SELECT * FROM students";

                    using (MySqlDataReader ResultSet = Command.ExecuteReader())
                    {
                        while (ResultSet.Read())
                        {
                            int Id = Convert.ToInt32(ResultSet["studentid"]);
                            string FirstName = ResultSet["studentfname"].ToString();
                            string LastName = ResultSet["studentlname"].ToString();
                            DateTime EnrollmentDate = Convert.ToDateTime(ResultSet["enroldate"]);
                            string StudentNumber = ResultSet["studentnumber"].ToString(); // Corrected here

                            Student CurrentStudent = new Student
                            {
                                studentid = Id,
                                studentfname = FirstName,
                                studentlname = LastName,
                                enroldate = EnrollmentDate,
                                studentnumber = StudentNumber // Corrected here
                            };
                            Students.Add(CurrentStudent);
                        }
                    }
                }
            }

            return Students;
        }

        /// <summary>
        /// Returns a Student in the database by their ID
        /// </summary>
        /// <example>
        /// GET api/Student/FindStudent/3 -> {"StudentID":3,"FirstName":"Jane","LastName":"Smith","EnrollmentDate":"2020-06-10","StudentNumber":"2001"}
        /// </example>
        /// <returns>
        /// A matching student object by its ID. Empty object if Student not found
        /// </returns>
        [HttpGet]
        [Route("FindStudent/{id}")]
        public Student FindStudent(int id)
        {
            Student? SelectedStudent = null;

            using (MySqlConnection Connection = (MySqlConnection)_context.Database.GetDbConnection())
            {
                Connection.Open();
                using (MySqlCommand Command = Connection.CreateCommand())
                {
                    Command.CommandText = "SELECT * FROM students WHERE StudentID=@id";
                    Command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader ResultSet = Command.ExecuteReader())
                    {
                        if (ResultSet.Read())
                        {
                            int Id = Convert.ToInt32(ResultSet["studentid"]);
                            string FirstName = ResultSet["studentfname"].ToString();
                            string LastName = ResultSet["studentlname"].ToString();
                            DateTime EnrollmentDate = Convert.ToDateTime(ResultSet["enroldate"]);
                            string StudentNumber = ResultSet["studentnumber"].ToString(); // Corrected here

                            SelectedStudent = new Student
                            {
                                studentid = Id,
                                studentfname = FirstName,
                                studentlname = LastName,
                                enroldate = EnrollmentDate,
                                studentnumber = StudentNumber // Corrected here
                            };
                        }
                    }
                }
            }

            return SelectedStudent;
        }

        /// <summary>
        /// Adds a new student to the system.
        /// </summary>
        [HttpPost]
        [Route("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest("Invalid student data.");
            }

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            return Ok("Student added successfully!");
        }

        /// <summary>
        /// Deletes a student by ID from the system.
        /// </summary>
        [HttpDelete]
        [Route("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok($"Student with ID {id} deleted successfully.");
        }
    }
}
