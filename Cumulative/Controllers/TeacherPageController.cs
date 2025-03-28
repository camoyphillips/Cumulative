using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System.Linq;
using System; // Required for StringComparison

namespace Cumulative.Controllers
{
    public class TeacherPageController(SchoolDbContext context) : Controller
    {
        private readonly SchoolDbContext _context = context;

        // GET: Teacher/List?{SearchKey}
        public IActionResult List(string SearchKey)
        {
            var teachers = string.IsNullOrEmpty(SearchKey)
                ? _context.Teachers.ToList()
                : _context.Teachers
                    .Where(teacher =>
                        teacher.FirstName.Contains(SearchKey, StringComparison.OrdinalIgnoreCase) ||
                        teacher.LastName.Contains(SearchKey, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            return View(teachers);
        }

        // GET: Teacher/Show/5
        public IActionResult Show(int id)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.TeacherID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
    }
}