// Cumulative/Controllers/TeacherAPIController.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;

namespace Cumulative.Controllers
{
    // Consistent Route: Use /api/teachers
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
        /// Returns a list of Teachers in the system, optionally filtered by search key.
        /// </summary>
        /// <param name="searchKey">Optional search key to filter by first or last name.</param>
        /// <returns>A list of Teacher objects.</returns>
        /// <example>GET /api/teachers</example>
        /// <example>GET /api/teachers?searchKey=Smith</example>
        [HttpGet(template:"ListTeachers")] // Route: GET /TeacherAPI
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Teacher>> ListTeachers([FromQuery] string? searchKey = null)
        {
            List<Teacher> ListTeachers = new List<Teacher>();
            string sqlQuery = "SELECT teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone FROM teachers";

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                // Add filtering logic safely
                sqlQuery += " WHERE LOWER(teacherfname) LIKE @searchKey OR LOWER(teacherlname) LIKE @searchKey";
            }
            sqlQuery += " ORDER BY teacherlname, teacherfname"; 

            try
            {
                using var Connection = _context.AccessDatabase();
                Connection.Open();
                using var Command = Connection.CreateCommand();
                Command.CommandText = sqlQuery;

                if (!string.IsNullOrWhiteSpace(searchKey))
                {
                    Command.Parameters.AddWithValue("@searchKey", $"%{searchKey.ToLower()}%"); // Use % for partial match
                }

                using var ResultSet = Command.ExecuteReader();
                while (ResultSet.Read())
                {
                    int Id = Convert.ToInt32(ResultSet["teacherid"]);
                    string FirstName = ResultSet["teacherfname"]?.ToString() ?? "";
                    string LastName = ResultSet["teacherlname"]?.ToString() ?? "";
                    string EmployeeNumber = ResultSet["employeenumber"]?.ToString() ?? "";
                    DateTime? HireDate = ResultSet["hiredate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(ResultSet["hiredate"]);
                    decimal Salary = Convert.ToDecimal(ResultSet["salary"]);
                    string? WorkPhone = ResultSet["teacherworkphone"] == DBNull.Value ? null : ResultSet["teacherworkphone"].ToString(); // Handle NULL

                    // Using the updated constructor for consistency
                    Teacher CurrentTeacher = new Teacher(Id, FirstName, LastName, EmployeeNumber, HireDate, Salary, WorkPhone);
                    ListTeachers.Add(CurrentTeacher);
                }
                return Ok(ListTeachers);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving teachers.");
            }
        }


        /// <summary>
        /// Finds a specific Teacher by their ID.
        /// </summary>
        /// <param name="id">The primary key of the teacher.</param>
        /// <returns>A Teacher object or NotFound.</returns>
        /// <example>GET /TeacherAPI/2</example>
        [HttpGet(template:"FindSelectedTeacher/{id}")] // Route: GET /TeacherAPI/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Teacher> FindSelectedTeacher(int id) 
        {
            Teacher? FoundTeacher = null;
            string sqlQuery = "SELECT teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone FROM teachers WHERE teacherid=@teacherid";

            try
            {
                using var Connection = _context.AccessDatabase();
                Connection.Open();
                using var Command = Connection.CreateCommand();
                Command.CommandText = sqlQuery;
                Command.Parameters.AddWithValue("@teachereid", id);

                using var ResultSet = Command.ExecuteReader();
                if (ResultSet.Read())
                {
                    int Id = Convert.ToInt32(ResultSet["teacherid"]);
                    string FirstName = ResultSet["teacherfname"]?.ToString() ?? "";
                    string LastName = ResultSet["teacherlname"]?.ToString() ?? "";
                    string EmployeeNumber = ResultSet["employeenumber"]?.ToString() ?? "";
                    DateTime? HireDate = ResultSet["hiredate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(ResultSet["hiredate"]);
                    decimal Salary = Convert.ToDecimal(ResultSet["salary"]);
                    string? WorkPhone = ResultSet["teacherworkphone"] == DBNull.Value ? null : ResultSet["teacherworkphone"].ToString();

                    FoundTeacher = new Teacher(Id, FirstName, LastName, EmployeeNumber, HireDate, Salary, WorkPhone);
                }

                if (FoundTeacher == null)
                {
                    return NotFound();
                }
                return Ok(FoundTeacher);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the teacher.");
            }
        }

        /// <summary>
        /// Adds a new teacher to the system. Expects JSON data.
        /// </summary>
        /// <param name="teacherData">A Teacher object containing the data for the new teacher.</param>
        /// <returns>The newly created Teacher object or BadRequest/Error.</returns>
        /// <example>
        /// POST /api/teachers
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///   "teacherfname": "Alice",
        ///   "teacherlname": "Jones",
        ///   "employeenumber": "T789",
        ///   "hiredate": "2024-05-10",
        ///   "salary": 58500.50,
        ///   "teacherworkphone": "416-555-0011"
        /// }
        /// </example>
        [HttpPost(template:"AddTeacher")] // Route: POST /TeacherAPI
        [Consumes("application/json")] // Expect JSON
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Teacher> AddTeacher([FromBody] Teacher teacherData)
        {
            // Server-side Validation (Leverage Model Attributes + Custom Logic)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns detailed validation errors
            }
            if (teacherData.hiredate.HasValue && teacherData.hiredate.Value.Date > DateTime.Now.Date) // Compare dates only
            {
                // Add error to ModelState to return structured error
                ModelState.AddModelError(nameof(teacherData.hiredate), "Hire date cannot be in the future.");
                return BadRequest(ModelState);
            }
            if (!Regex.IsMatch(teacherData.employeenumber ?? "", @"^T\d+$"))
            {
                ModelState.AddModelError(nameof(teacherData.employeenumber), "Employee number must start with 'T' followed by digits.");
                return BadRequest(ModelState);
            }
            // Salary validation handled by [Range] attribute on model

            try
            {
                using var Connection = _context.AccessDatabase();
                Connection.Open();

                // Check for duplicate employee number (essential for unique constraint)
                using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM teachers WHERE employeenumber = @employeeenumber AND teacherid != @teacherid", Connection)) 
                {
                    // For Add, ID check isn't needed, but good pattern for Update
                    checkCmd.Parameters.AddWithValue("@employeenumber", teacherData.employeenumber ?? "");
                    checkCmd.Parameters.AddWithValue("@teacherid", 0); // For Add, no existing ID
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        ModelState.AddModelError(nameof(teacherData.employeenumber), "Employee number already exists.");
                        return BadRequest(ModelState);
                    }
                }

                // Add teacherworkphone to INSERT
                var query = @"INSERT INTO teachers
                              (teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone)
                              VALUES
                              (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary, @teacherworkphone);
                              SELECT LAST_INSERT_ID();"; // Get the ID back efficiently

                using var Command = Connection.CreateCommand();
                Command.CommandText = query;

                Command.Parameters.AddWithValue("@teacherfname", teacherData.teacherfname ?? "");
                Command.Parameters.AddWithValue("@teacherlname", teacherData.teacherlname ?? "");
                Command.Parameters.AddWithValue("@employeenumber", teacherData.employeenumber ?? "");
                Command.Parameters.AddWithValue("@hiredate", teacherData.hiredate ?? (object)DBNull.Value);
                Command.Parameters.AddWithValue("@salary", teacherData.salary);
                Command.Parameters.AddWithValue("@teacherworkphone", (object?)teacherData.teacherworkphone ?? DBNull.Value); // Handle null

                // ExecuteScalar to get the LastInsertedId
                int newTeacherId = Convert.ToInt32(Command.ExecuteScalar());
                teacherData.teacherid = newTeacherId; // Set the ID on the object

                // Return 201 Created with the location and the created object
                return CreatedAtAction(nameof(FindSelectedTeacher), new { id = newTeacherId }, teacherData);

            }
            catch (MySqlException ex) when (ex.Number == 1062) // Catch specific duplicate entry error if constraint exists
            {
                ModelState.AddModelError(nameof(teacherData.employeenumber), "Employee number already exists (database constraint).");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding teacher: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing teacher in the system. Expects JSON data.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="teacherData">A Teacher object containing the updated data.</param>
        /// <returns>NoContent if successful, NotFound, BadRequest, or Error.</returns>
        /// <example>
        /// PUT /TeacherAPI/2
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///   "teacherid": 2, // ID in body might be ignored or validated against URL id
        ///   "teacherfname": "Caitlin Updated",
        ///   "teacherlname": "Smith",
        ///   "employeenumber": "T456",
        ///   "hiredate": "2022-08-20",
        ///   "salary": 63000.00,
        ///   "teacherworkphone": "555-111-2222"
        /// }
        /// </example>
        [HttpPut(template:"UpdateTeacher/{id}")] // Route: PUT /TeacherAPI/UpdateTeacher{id}
        [Consumes("application/json")] // Expect JSON
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher teacherData)
        {
            // Optional: Validate that the ID in the route matches the ID in the body if provided
            if (teacherData.teacherid != 0 && teacherData.teacherid != id)
            {
                ModelState.AddModelError(nameof(teacherData.teacherid), "Teacher ID in body does not match route ID.");
                return BadRequest(ModelState);
            }

            // Server-side Validation (Leverage Model Attributes + Custom Logic)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (teacherData.hiredate.HasValue && teacherData.hiredate.Value.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError(nameof(teacherData.hiredate), "Hire date cannot be in the future.");
                return BadRequest(ModelState);
            }
            if (!Regex.IsMatch(teacherData.employeenumber ?? "", @"^T\d+$"))
            {
                ModelState.AddModelError(nameof(teacherData.employeenumber), "Employee number must start with 'T' followed by digits.");
                return BadRequest(ModelState);
            }
            // Salary validation handled by [Range]

            try
            {
                using var Connection = _context.AccessDatabase();
                Connection.Open();

                // 1. Check if Teacher Exists
                using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM teachers WHERE teacherid = @teacherid", Connection))
                {
                    checkCmd.Parameters.AddWithValue("@id", id);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (exists == 0)
                    {
                        return NotFound($"Teacher with ID {id} not found."); // Initiative Mark: Server Error Handling on Update when trying to update a teacher that does not exist
                    }
                }

                // 2. Check for duplicate employee number (excluding the current teacher)
                using (var checkEmpCmd = new MySqlCommand("SELECT COUNT(*) FROM teachers WHERE employeenumber = @employeenumber AND teacherid != @teacherid", Connection))
                {
                    checkEmpCmd.Parameters.AddWithValue("@employeenumber", teacherData.employeenumber ?? "");
                    checkEmpCmd.Parameters.AddWithValue("@teacherid", id);
                    int count = Convert.ToInt32(checkEmpCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        ModelState.AddModelError(nameof(teacherData.employeenumber), "Employee number already exists for another teacher.");
                        return BadRequest(ModelState);
                    }
                }

                // 3. Perform Update
                var query = @"UPDATE teachers SET
                                teacherfname = @teacherfname,
                                teacherlname = @teacherlname,
                                employeenumber = @employeenumber,
                                hiredate = @hiredate,
                                salary = @salary,
                                teacherworkphone = @teacherworkphone
                              WHERE teacherid = @teacherid";

                using var Command = Connection.CreateCommand();
                Command.CommandText = query;

                Command.Parameters.AddWithValue("@teacherfname", teacherData.teacherfname ?? "");
                Command.Parameters.AddWithValue("@teacherlname", teacherData.teacherlname ?? "");
                Command.Parameters.AddWithValue("@employeenumber", teacherData.employeenumber ?? "");
                Command.Parameters.AddWithValue("@hiredate", teacherData.hiredate ?? (object)DBNull.Value);
                Command.Parameters.AddWithValue("@salary", teacherData.salary);
                Command.Parameters.AddWithValue("@teacherworkphone", (object?)teacherData.teacherworkphone ?? DBNull.Value);
                Command.Parameters.AddWithValue("@id", id); // Use id from route parameter

                int rowsAffected = Command.ExecuteNonQuery();

                // It's possible ExecuteNonQuery returns 0 if the data submitted was identical
                // to the existing data. Typically, we still consider this a success for PUT.
                // If rowsAffected == 0 AND the check at the beginning passed, it means
                // either no change or ID not found (covered by the initial check).
                // So, returning NoContent is appropriate here.

                return NoContent(); // Standard success response for PUT

            }
            catch (MySqlException ex) when (ex.Number == 1062) // Catch specific duplicate entry error
            {
                ModelState.AddModelError(nameof(teacherData.employeenumber), "Employee number already exists (database constraint).");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating teacher: {ex.Message}");
            }
        }


        /// <summary>
        /// Deletes a teacher by ID and handles related course references.
        /// </summary>
        /// <param name="id">The primary key of the teacher to delete.</param>
        /// <returns>NoContent if successful, NotFound or Error.</returns>
        /// <example>DELETE /api/teachers/2</example>
        [HttpDelete(template:"DeleteTeacher/{id}")] // Route: DELETE /TeacherAPI/DeleteTeacher{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTeacher(int id)
        {
            // **Important:** Choose ONE strategy for handling related data.
            // Strategy 1: Rely on Database Constraint (ON DELETE SET NULL)
            // If you have set up the foreign key constraint in your Courses table like:
            // FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID) ON DELETE SET NULL
            // Then you only need to perform the delete operation here.

            // Strategy 2: Server-Side Logic (Manual Update)
            // If you don't have the DB constraint, do this *before* deleting the teacher.
            /*
            string updateCoursesQuery = "UPDATE courses SET TeacherID = NULL WHERE TeacherID = @id_to_delete";
            */

            MySqlConnection? Connection = null; // Declare outside try
            MySqlTransaction? transaction = null; // For atomicity

            try
            {
                Connection = _context.AccessDatabase();
                Connection.Open();
                transaction = Connection.BeginTransaction(); // Start transaction

                // === Use EITHER DB Constraint OR Server Logic ===

                // --- Server Logic Example (if NO DB constraint) ---
                // Comment this out if using ON DELETE SET NULL constraint
                /*
                using (var updateCmd = new MySqlCommand(updateCoursesQuery, Connection, transaction)) // Associate with transaction
                {
                    updateCmd.Parameters.AddWithValue("@id_to_delete", id);
                    updateCmd.ExecuteNonQuery(); // Set related courses' TeacherID to NULL
                }
                */
                // --- End Server Logic Example ---


                // Now, delete the teacher
                using var deleteCmd = new MySqlCommand("DELETE FROM teachers WHERE teacherid=@teacherid", Connection, transaction); // Associate with transaction
                deleteCmd.Parameters.AddWithValue("@teacherid", id);
                int rowsAffected = deleteCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    transaction.Commit(); // Commit transaction if delete was successful
                    return NoContent(); // Standard success response for DELETE
                }
                else
                {
                    transaction.Rollback(); // Rollback if teacher not found
                    return NotFound($"No Teacher found with ID {id}.");
                }
            }
            catch (MySqlException mex) // Catch potential FK constraint errors if not using SET NULL/CASCADE correctly elsewhere
            {
                transaction?.Rollback(); // Rollback on error
                                         // Log the exception mex
                                         // Check mex.Number for specific FK errors if needed
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error deleting teacher. Check related records (e.g., courses): {mex.Message}");
            }
            catch (Exception ex)
            {
                transaction?.Rollback(); // Rollback on general error
                                         // Log the exception ex
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting teacher: {ex.Message}");
            }
            finally // Ensure connection is closed even if transaction wasn't created
            {
                Connection?.Close();
            }
        }
    }
}