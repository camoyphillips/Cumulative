using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Cumulative.Controllers
{
    [Route("api/Teacher")]
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
        /// <example>
        /// GET api/Teacher/ListTeachers -> [{"TeacherID":1,"FirstName":"John", "LastName":"Doe", "EmployeeNumber":"T123", "HireDate":"2023-01-15", "Salary":50000.00},...]
        /// </example>
        /// <returns>
        /// A list of teacher objects 
        /// </returns>
        [HttpGet]
        [Route("ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            List<Teacher> Teachers = new List<Teacher>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "SELECT * FROM teachers";

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        string EmployeeNumber = ResultSet["employeenumber"].ToString();
                        DateTime? HireDate = ResultSet["hiredate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["HireDate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                        Teacher CurrentTeacher = new Teacher(Id, FirstName, LastName, EmployeeNumber, HireDate, Salary);
                        Teachers.Add(CurrentTeacher);
                    }
                }
            }

            return Teachers;
        }

        /// <summary>
        /// Returns a Teacher in the database by their ID
        /// </summary>
        /// <example>
        /// GET api/Teacher/FindTeacher/3 -> {"TeacherID":3,"FirstName":"Jane","LastName":"Smith","EmployeeNumber":"T456","HireDate":"2020-06-10","Salary":60000.00}
        /// </example>
        /// <returns>
        /// A matching teacher object by its ID. Empty object if Teacher not found
        /// </returns>
        [HttpGet]
        [Route("FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher SelectedTeacher = null;

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "SELECT * FROM teachers WHERE TeacherID=@id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    if (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();
                        string EmployeeNumber = ResultSet["employeenumber"].ToString();
                        DateTime? HireDate = ResultSet["hiredate"] == DBNull.Value ? null : Convert.ToDateTime(ResultSet["HireDate"]);
                        decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                        SelectedTeacher = new Teacher(Id, FirstName, LastName, EmployeeNumber, HireDate, Salary);
                    }
                }
            }

            return SelectedTeacher;
        }
    }
}
