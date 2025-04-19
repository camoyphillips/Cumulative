using Microsoft.AspNetCore.Mvc;
using Cumulative.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Cumulative.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly SchoolDbContext _context;

        public TeacherPageController(SchoolDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: TeacherPage/ListTeachers
        [HttpGet]
        public IActionResult ListTeachers(string SearchKey)
        {
            ViewBag.SearchKey = SearchKey;

            List<Teacher> ListTeachers = string.IsNullOrWhiteSpace(SearchKey)
                ? _context.Teachers.ToList()
                : _context.Teachers
                    .Where(t =>
                        (t.teacherfname != null && t.teacherfname.Contains(SearchKey, StringComparison.OrdinalIgnoreCase)) ||
                        (t.teacherlname != null && t.teacherlname.Contains(SearchKey, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

            return View(ListTeachers);
        }

        // GET: TeacherPage/FindSelectedTeacher/5
        [HttpGet]
        public IActionResult FindSelectedTeacher(int id)
        {
            var FindSelectedTeacher = _context.Teachers.FirstOrDefault(t => t.teacherid == id);
            return FindSelectedTeacher == null ? NotFound() : View(FindSelectedTeacher);
        }

        // GET: TeacherPage/New
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        // POST: TeacherPage/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(Teacher newTeacher)
        {
            if (ModelState.IsValid)
            {
                _context.Teachers.Add(newTeacher);
                _context.SaveChanges();

                return RedirectToAction("FindSelectedTeacher", new { id = newTeacher.teacherid });
            }

            return View(newTeacher);
        }

        // GET: TeacherPage/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var teacherToEdit = _context.Teachers.FirstOrDefault(t => t.teacherid == id);
            return teacherToEdit == null ? NotFound() : View(teacherToEdit);
        }

        // POST: TeacherPage/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, Teacher updatedTeacher)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", updatedTeacher);
            }

            var existingTeacher = _context.Teachers.FirstOrDefault(t => t.teacherid == id);
            if (existingTeacher == null)
            {
                return NotFound();
            }

            existingTeacher.teacherfname = updatedTeacher.teacherfname;
            existingTeacher.teacherlname = updatedTeacher.teacherlname;
            existingTeacher.employeenumber = updatedTeacher.employeenumber;
            existingTeacher.hiredate = updatedTeacher.hiredate;
            existingTeacher.salary = updatedTeacher.salary;

            _context.SaveChanges();

            return RedirectToAction("FindSelectedTeacher", new { id = existingTeacher.teacherid });
        }

        // GET: TeacherPage/DeleteConfirm/5
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            var FindSelectedTeacher = _context.Teachers.FirstOrDefault(t => t.teacherid == id);
            return FindSelectedTeacher == null ? NotFound() : View(FindSelectedTeacher);
        }

        // POST: TeacherPage/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var TeacherToDelete = _context.Teachers.FirstOrDefault(t => t.teacherid == id);
            if (TeacherToDelete == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(TeacherToDelete);
            _context.SaveChanges();

            return RedirectToAction("ListTeachers");
        }
    }
}
