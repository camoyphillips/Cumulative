using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cumulative.Controllers
{
    [Route("api/Course")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public CourseAPIController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Courses in the system
        /// </summary>
        /// <example>
        /// GET api/Course/ListCourses -> [{"CourseID":1,"CourseCode":"CS101", "CourseName":"Intro to Computer Science", "TeacherID":1, "StartDate":"2023-01-15", "FinishDate":"2023-05-15"},...]
        /// </example>
        /// <returns>
        /// A list of course objects
        /// </returns>
        [HttpGet]
        [Route("ListCourses")]
        public ActionResult<List<Course>> ListCourses()
        {
            try
            {
                List<Course> Courses = new List<Course>();

                using (MySqlConnection Connection = (MySqlConnection)_context.Database.GetDbConnection())
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = "SELECT * FROM courses";

                    using (MySqlDataReader ResultSet = Command.ExecuteReader())
                    {
                        while (ResultSet.Read())
                        {
                            int Id = Convert.ToInt32(ResultSet["courseid"]);
                            string CourseCode = ResultSet["coursecode"].ToString();
                            string CourseName = ResultSet["coursename"].ToString();
                            int TeacherID = Convert.ToInt32(ResultSet["teacherid"]);
                            DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                            DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);

                            Course CurrentCourse = new Course(Id, CourseCode, CourseName, TeacherID, StartDate, FinishDate);
                            Courses.Add(CurrentCourse);
                        }
                    }
                }

                return Ok(Courses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving courses: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a Course in the database by its ID
        /// </summary>
        /// <example>
        /// GET api/Course/FindCourse/3 -> {"CourseID":3,"CourseCode":"CS202","CourseName":"Data Structures","TeacherID":2,"StartDate":"2022-08-01","FinishDate":"2022-12-15"}
        /// </example>
        /// <returns>
        /// A matching course object by its ID. Empty object if Course not found
        /// </returns>
        [HttpGet]
        [Route("FindCourse/{id}")]
        public ActionResult<Course> FindCourse(int id)
        {
            try
            {
                Course SelectedCourse = null;

                using (MySqlConnection Connection = (MySqlConnection)_context.Database.GetDbConnection())
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = "SELECT * FROM courses WHERE CourseID=@id";
                    Command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader ResultSet = Command.ExecuteReader())
                    {
                        if (ResultSet.Read())
                        {
                            int Id = Convert.ToInt32(ResultSet["courseid"]);
                            string CourseCode = ResultSet["coursecode"].ToString();
                            string CourseName = ResultSet["coursename"].ToString();
                            int TeacherID = Convert.ToInt32(ResultSet["teacherid"]);
                            DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                            DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);

                            SelectedCourse = new Course(Id, CourseCode, CourseName, TeacherID, StartDate, FinishDate);
                        }
                    }
                }

                if (SelectedCourse == null)
                {
                    return NotFound();
                }

                return Ok(SelectedCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving course: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new Course to the system
        /// </summary>
        /// <param name="course">Course object to add</param>
        /// <returns>Returns the added course object</returns>
        [HttpPost]
        [Route("AddCourse")]
        public IActionResult AddCourse([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest("Course data is null");
            }

            try
            {
                using (MySqlConnection Connection = (MySqlConnection)_context.Database.GetDbConnection())
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = "INSERT INTO courses (CourseCode, CourseName, TeacherID, StartDate, FinishDate) VALUES (@CourseCode, @CourseName, @TeacherID, @StartDate, @FinishDate)";
                    Command.Parameters.AddWithValue("@CourseCode", course.CourseCode);
                    Command.Parameters.AddWithValue("@CourseName", course.CourseName);
                    Command.Parameters.AddWithValue("@TeacherID", course.TeacherID);
                    Command.Parameters.AddWithValue("@StartDate", course.StartDate);
                    Command.Parameters.AddWithValue("@FinishDate", course.FinishDate);

                    int result = Command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return CreatedAtAction("FindCourse", new { id = course.CourseID }, course);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error creating the course");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating course: {ex.Message}");
            }
        }
    }
}