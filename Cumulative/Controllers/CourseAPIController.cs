using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System;
using System.Collections.Generic;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cumulative.Controllers
{
    [Route("CourseAPI")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public CourseAPIController(SchoolDbContext context)
        {
            _context = context;
        }

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
                    Command.Parameters.AddWithValue("@CourseCode", course.coursecode);
                    Command.Parameters.AddWithValue("@CourseName", course.coursename);
                    Command.Parameters.AddWithValue("@TeacherID", course.teacherid);
                    Command.Parameters.AddWithValue("@StartDate", course.startdate);
                    Command.Parameters.AddWithValue("@FinishDate", course.finishdate);

                    int result = Command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return CreatedAtAction("FindCourse", new { id = course.courseid }, course);
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

        [HttpPut]
        [Route("UpdateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] Course updatedCourse)
        {
            if (updatedCourse == null || id != updatedCourse.courseid)
            {
                return BadRequest("Invalid course data.");
            }

            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            existingCourse.coursecode = updatedCourse.coursecode;
            existingCourse.coursename = updatedCourse.coursename;
            existingCourse.teacherid = updatedCourse.teacherid;
            existingCourse.startdate = updatedCourse.startdate;
            existingCourse.finishdate = updatedCourse.finishdate;

            _context.Courses.Update(existingCourse);
            await _context.SaveChangesAsync();

            return Ok("Course updated successfully.");
        }

        [HttpDelete]
        [Route("DeleteCourse/{id}")]
        public IActionResult DeleteCourse(int id)
        {
            try
            {
                using (MySqlConnection Connection = (MySqlConnection)_context.Database.GetDbConnection())
                {
                    Connection.Open();
                    MySqlCommand Command = Connection.CreateCommand();
                    Command.CommandText = "DELETE FROM courses WHERE CourseID = @id";
                    Command.Parameters.AddWithValue("@id", id);

                    int result = Command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok($"Course with ID {id} deleted successfully.");
                    }
                    else
                    {
                        return NotFound($"No course found with ID {id}.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting course: {ex.Message}");
            }
        }
    }
}
