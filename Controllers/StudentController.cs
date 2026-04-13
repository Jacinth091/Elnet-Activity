using Barral_ELNET1_MVC.Data;
using Barral_ELNET1_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barral_ELNET1_MVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        // Static list of full course names
        private readonly List<string> _fullCourseNames = new List<string>
        {
            "Bachelor of Science in Information Technology",
            "Bachelor of Science in Computer Science",
            "Bachelor of Science in Information Systems",
            "Bachelor of Science in Accountancy",
            "Bachelor of Science in Nursing",
            "Bachelor of Science in Business Administration",
            "Bachelor of Secondary Education"
        };

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, string? courseFilter)
        {
            var query = _context.Students.AsQueryable();

            // Search by name/email/id (case-insensitive)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearch = searchTerm.Trim().ToLower();
                var parsedId = int.TryParse(normalizedSearch, out var idValue);

                query = query.Where(s =>
                    s.Name.ToLower().Contains(normalizedSearch) ||
                    s.Email.ToLower().Contains(normalizedSearch) ||
                    (parsedId && s.Id == idValue));
            }

            // Filter by course/program
            if (!string.IsNullOrWhiteSpace(courseFilter))
            {
                var normalizedCourse = courseFilter.Trim().ToLower();
                query = query.Where(s => s.Course.ToLower() == normalizedCourse);
            }

            var students = await query
                .OrderBy(s => s.Id)
                .ToListAsync();

            var courseOptions = await _context.Students
                .Select(s => s.Course)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CourseFilter = courseFilter;
            ViewBag.CourseOptions = courseOptions;

            return View(students);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CourseOptionsList = _fullCourseNames;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Student student)
        {
            if (await _context.Students.AnyAsync(s => s.Name == student.Name))
            {
                ModelState.AddModelError("Name", "A student with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student record created successfully.";
                return RedirectToAction("Index");
            }
            ViewBag.CourseOptionsList = _fullCourseNames;
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewBag.CourseOptionsList = _fullCourseNames;
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Student student)
        {
            if (await _context.Students.AnyAsync(s => s.Name == student.Name && s.Id != student.Id))
            {
                ModelState.AddModelError("Name", "A student with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Students.Update(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student record updated successfully.";
                return RedirectToAction("Index");
            }
            ViewBag.CourseOptionsList = _fullCourseNames;
            return View(student);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
                TempData["Success"] = "Student record deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Error: Student record not found.";
            }
            return RedirectToAction("Index");
        }
    }
}