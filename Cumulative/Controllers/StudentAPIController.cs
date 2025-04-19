using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System;
using System.Collections.Generic;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cumulative.Controllers
{
    [Route("StudentAPI")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StudentAPIController(SchoolDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

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
                            string StudentNumber = ResultSet["studentnumber"].ToString();

                            Student CurrentStudent = new Student
                            {
                                studentid = Id,
                                studentfname = FirstName,
                                studentlname = LastName,
                                enroldate = EnrollmentDate,
                                studentnumber = StudentNumber
                            };
                            Students.Add(CurrentStudent);
                        }
                    }
                }
            }

            return Students;
        }

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
                    Command.CommandText = "SELECT * FROM students WHERE studentid=@id";
                    Command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader ResultSet = Command.ExecuteReader())
                    {
                        if (ResultSet.Read())
                        {
                            int Id = Convert.ToInt32(ResultSet["studentid"]);
                            string FirstName = ResultSet["studentfname"].ToString();
                            string LastName = ResultSet["studentlname"].ToString();
                            DateTime EnrollmentDate = Convert.ToDateTime(ResultSet["enroldate"]);
                            string StudentNumber = ResultSet["studentnumber"].ToString();

                            SelectedStudent = new Student
                            {
                                studentid = Id,
                                studentfname = FirstName,
                                studentlname = LastName,
                                enroldate = EnrollmentDate,
                                studentnumber = StudentNumber
                            };
                        }
                    }
                }
            }

            return SelectedStudent ?? new Student();
        }

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

            return Ok("Student added successfully.");
        }

        [HttpPut]
        [Route("UpdateStudent/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            if (updatedStudent == null || id != updatedStudent.studentid)
            {
                return BadRequest("Invalid student data.");
            }

            var existingStudent = await _context.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            existingStudent.studentfname = updatedStudent.studentfname;
            existingStudent.studentlname = updatedStudent.studentlname;
            existingStudent.enroldate = updatedStudent.enroldate;
            existingStudent.studentnumber = updatedStudent.studentnumber;

            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();

            return Ok("Student updated successfully.");
        }

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
