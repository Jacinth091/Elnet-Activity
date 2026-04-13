using Barral_ELNET1_MVC.Data;
using Barral_ELNET1_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var admins = await _context.Users.Where(u => u.Role == "Admin").ToListAsync();
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.CurrentAdminId = int.TryParse(nameIdentifier, out int id) ? id : 0;
            return View(admins);
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
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
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
            return RedirectToAction("Index");
        }

        // GET: Admin/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.Role != "Admin")
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var userToUpdate = await _context.Users.FindAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            // Remove password validation if it's not being changed
            if (string.IsNullOrWhiteSpace(model.Password) && string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }
            
            if (ModelState.IsValid)
            {
                userToUpdate.Name = model.Name;
                userToUpdate.Email = model.Email;
                userToUpdate.Age = model.Age;
                userToUpdate.BirthDate = model.BirthDate;

                // Hash the new password if it was provided
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    userToUpdate.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                try
                {
                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Id == userToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "Admin account updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentAdminId = int.TryParse(nameIdentifier, out int cid) ? id : 0;
            if (id == cid)
            {
                TempData["Error"] = "Error: You cannot delete your own account.";
                return RedirectToAction(nameof(Index));
            }

            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete != null && userToDelete.Role == "Admin")
            {
                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Admin account deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Admin not found or user is not an admin.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
