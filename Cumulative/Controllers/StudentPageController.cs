using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cumulative.Controllers
{
    public class StudentPageController : Controller
    {
        private readonly SchoolDbContext _context;

        // Constructor using Dependency Injection
        public StudentPageController(SchoolDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of all student names in the system.
        /// </summary>
        /// <example>
        /// GET /Student/ListStudentNames -> ["Alice Johnson", "Bob Smith", ...]
        /// </example>
        /// <returns>A list of strings formatted "{FirstName} {LastName}"</returns>
        public async Task<List<string>> ListStudentNames()
        {
            var studentNames = await _context.Students
                .Where(s => !string.IsNullOrEmpty(s.FirstName) && !string.IsNullOrEmpty(s.LastName))
                .Select(s => $"{s.FirstName} {s.LastName}")
                .ToListAsync();

            return studentNames;
        }

        /// <summary>
        /// Finds a specific student by their ID.
        /// </summary>
        /// <example>
        /// GET /Student/FindStudent/1 -> { Id: 1, FirstName: "Alice", ... }
        /// </example>
        /// <param name="id">The student ID</param>
        /// <returns>A student object</returns>
        public async Task<IActionResult> FindStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
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

        /// <summary>
        /// The Index action to fetch all students and pass them to the view.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.ToListAsync();
            return View(students);
        }
    }
}
