using Cumulative.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Cumulative.Controllers
{
    public class TeacherAjaxPageController : Controller
    {
        private readonly string connectionString = "server=localhost;username=root;password=root;database=school_db;";

        // Load Teacher List View
        [HttpGet]
        public IActionResult TeacherList()
        {
            List<Teacher> teachers = new List<Teacher>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM teachers", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    teachers.Add(new Teacher
                    {
                        teacherid = Convert.ToInt32(reader["teacherid"]),
                        teacherfname = reader["teacherfname"].ToString(),
                        teacherlname = reader["teacherlname"].ToString(),
                        employeenumber = reader["employeenumber"].ToString(),
                        hiredate = Convert.ToDateTime(reader["hiredate"]),
                        salary = reader["salary"] != DBNull.Value ? Convert.ToDecimal(reader["salary"]) : 0,
                        teacherworkphone = reader["teacherworkphone"].ToString()
                    });
                }
            }

            return View(teachers);
        }

        // AJAX: Add Teacher
        [HttpPost]
        public JsonResult AddTeacher([FromBody] Teacher newTeacher)
        {
            if (newTeacher == null)
            {
                return Json(new { success = false, message = "Invalid teacher data." });
            }

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var query = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone) " +
                            "VALUES (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary, @teacherworkphone)";

                var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@teacherfname", newTeacher.teacherfname);
                cmd.Parameters.AddWithValue("@teacherlname", newTeacher.teacherlname);
                cmd.Parameters.AddWithValue("@employeenumber", newTeacher.employeenumber);
                cmd.Parameters.AddWithValue("@hiredate", newTeacher.hiredate);
                cmd.Parameters.AddWithValue("@salary", newTeacher.salary);
                cmd.Parameters.AddWithValue("@teacherworkphone", newTeacher.teacherworkphone);

                try
                {
                    cmd.ExecuteNonQuery();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        // AJAX: Delete Teacher
        [HttpPost]
        public JsonResult DeleteTeacher(int id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM teachers WHERE teacherid = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Teacher not found." });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }
}
