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
            var ListStudents = await _context.Students
                .Where(s => !string.IsNullOrEmpty(s.studentfname) && !string.IsNullOrEmpty(s.studentlname))
                .Select(s => $"{s.studentfname} {s.studentlname}")
                .ToListAsync();

            return ListStudents;
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
            var FindSelectedStudent = await _context.Students.FindAsync(id);

            if (FindSelectedStudent == null)
            {
                return NotFound();
            }

            return View(FindSelectedStudent);
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
