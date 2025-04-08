using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace Cumulative.Controllers
{
    [Route("TeacherAPI")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Teachers in the system
        /// </summary>
        [HttpGet(template:"ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            var Teachers = new List<Teacher>();

            using var Connection = _context.AccessDatabase();
            Connection.Open();
            using var Command = Connection.CreateCommand();
            Command.CommandText = "SELECT * FROM teachers";

            using var ResultSet = Command.ExecuteReader();
            while (ResultSet.Read())
            {
                int Id = Convert.ToInt32(ResultSet["teacherid"]);
                string FirstName = ResultSet["teacherfname"]?.ToString() ?? "";
                string LastName = ResultSet["teacherlname"]?.ToString() ?? "";
                string EmployeeNumber = ResultSet["employeenumber"]?.ToString() ?? "";
                DateTime? HireDate = ResultSet["hiredate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(ResultSet["hiredate"]);
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                var CurrentTeacher = new Teacher(Id, FirstName, LastName, EmployeeNumber, HireDate, Salary);
                Teachers.Add(CurrentTeacher);
            }

            return Teachers;
        }

        /// <summary>
        /// Finds a Teacher by ID
        /// </summary>
        [HttpGet(template:"FindSelectedTeacher/{id}")]
        public Teacher? FindSelectedTeacher(int id)
        {
            Teacher? FoundTeacher = null;

            using var Connection = _context.AccessDatabase();
            Connection.Open();
            using var Command = Connection.CreateCommand();
            Command.CommandText = "SELECT * FROM teachers WHERE teacherid=@id";
            Command.Parameters.AddWithValue("@id", id);

            using var ResultSet = Command.ExecuteReader();
            if (ResultSet.Read())
            {
                int Id = Convert.ToInt32(ResultSet["teacherid"]);
                string FirstName = ResultSet["teacherfname"]?.ToString() ?? "";
                string LastName = ResultSet["teacherlname"]?.ToString() ?? "";
                string EmployeeNumber = ResultSet["employeenumber"]?.ToString() ?? "";
                DateTime? HireDate = ResultSet["hiredate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(ResultSet["hiredate"]);
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                FoundTeacher = new Teacher(Id, FirstName, LastName, EmployeeNumber, HireDate, Salary);
            }

            return FoundTeacher;
        }

        /// <summary>
        /// Adds a new teacher to the system
        /// </summary>
        [HttpPost(template:"AddTeacher")]
        [Consumes("application/X-www-form-urlencoded")]
        public IActionResult AddTeacher([FromBody] Teacher TeacherData)
        {
            if (string.IsNullOrWhiteSpace(TeacherData.teacherfname))
                return BadRequest("Teacher first name is required.");

            if (string.IsNullOrWhiteSpace(TeacherData.teacherlname))
                return BadRequest("Teacher last name is required.");

            if (TeacherData.hiredate.HasValue && TeacherData.hiredate.Value > DateTime.Now)
                return BadRequest("Hire date cannot be in the future.");

            if (!Regex.IsMatch(TeacherData.employeenumber ?? "", @"^T\d+$"))
                return BadRequest("Employee number must start with 'T' followed by digits.");

            try
            {
                using var Connection = _context.AccessDatabase();
                Connection.Open();

                // Check for duplicate employee number
                using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM teachers WHERE employeenumber = @emp", Connection))
                {
                    checkCmd.Parameters.AddWithValue("@emp", TeacherData.employeenumber ?? "");
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                        return BadRequest("Employee number already exists.");
                }

                var query = @"INSERT INTO teachers 
                            (teacherfname, teacherlname, employeenumber, hiredate, salary) 
                            VALUES 
                            (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary)";

                using var Command = Connection.CreateCommand();
                Command.CommandText = query;

                Command.Parameters.AddWithValue("@teacherfname", TeacherData.teacherfname ?? "");
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.teacherlname ?? "");
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.employeenumber ?? "");
                Command.Parameters.AddWithValue("@hiredate", TeacherData.hiredate ?? (object)DBNull.Value);
                Command.Parameters.AddWithValue("@salary", TeacherData.salary);

                Command.ExecuteNonQuery();

                int teacherid = (int)Command.LastInsertedId;
                return Ok($"Inserted Teacher #{teacherid} successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding teacher: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a teacher by ID
        /// </summary>
        [HttpDelete(template:"DeleteTeacher/{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            try
            {
                using var Connection = _context.AccessDatabase();
                Connection.Open();

                using var Command = Connection.CreateCommand();
                Command.CommandText = "DELETE FROM teachers WHERE teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);

                int rowsAffected = Command.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return Ok($"Deleted Teacher with ID {id} successfully.");
                else
                    return NotFound($"No Teacher found with ID {id}.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting teacher: {ex.Message}");
            }
        }
    }
}
