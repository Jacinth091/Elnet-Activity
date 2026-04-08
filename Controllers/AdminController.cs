using Barral_ELNET1_MVC.Data;
using Barral_ELNET1_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Barral_ELNET1_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new User { BirthDate = DateOnly.FromDateTime(DateTime.Today) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model)
        {
            // Explicitly validate common requirements for Admin creation
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "An admin with this email already exists.");
                return View(model);
            }

            var adminUser = new User
            {
                Name = model.Name,
                Email = model.Email,
                Age = model.Age,
                BirthDate = model.BirthDate,
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = "New Admin account created successfully.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
