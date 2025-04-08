using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cumulative.Controllers
{
    public class CoursePageController : Controller
    {
        private readonly SchoolDbContext _context;

        // Constructor using Dependency Injection for SchoolDbContext
        public CoursePageController(SchoolDbContext context)
        {
            _context = context;
        }

        // GET: CoursePageController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Courses = await _context.Courses.ToListAsync();
            return View(Courses);
        }

        // GET: CoursePageController/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int courseID)
        {
            var SelectedCourse = await _context.Courses.FirstOrDefaultAsync(c => c.courseid == courseID);

            if (SelectedCourse == null)
            {
                return NotFound();
            }

            return View(SelectedCourse);
        }

        // GET: CoursePageController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CoursePageController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseName, CourseCode, TeacherID, StartDate, FinishDate")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: CoursePageController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int courseID)
        {
            var course = await _context.Courses.FindAsync(courseID);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: CoursePageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int courseID, [Bind("CourseID, CourseName, CourseCode, TeacherID, StartDate, FinishDate")] Course course)
        {
            if (courseID != course.courseid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.courseid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: CoursePageController/Delete/5
        public async Task<IActionResult> Delete(int courseID)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.courseid == courseID);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: CoursePageController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int courseID)
        {
            var course = await _context.Courses.FindAsync(courseID);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int courseID)
        {
            return _context.Courses.Any(e => e.courseid == courseID);
        }
    }
}
