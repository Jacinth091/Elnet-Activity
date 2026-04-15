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
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required for new accounts.");
            }

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

            // Explicitly remove password validation if the fields are left blank
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.Remove("Password");
            }
            if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
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
        // ──────────────────────────────────────────────────────────────────
        // GUEST MANAGEMENT
        // ──────────────────────────────────────────────────────────────────

        // GET: /Admin/Guests
        [HttpGet]
        public async Task<IActionResult> Guests()
        {
            var guests = await _context.Users
                .AsNoTracking()
                .Where(u => u.Role == "Guest" || u.Role == "GuestInactive")
                .OrderByDescending(u => u.Id)
                .ToListAsync();

            ViewBag.CreateForm = new User { BirthDate = DateOnly.FromDateTime(DateTime.Today) };
            return View(guests);
        }

        // POST: /Admin/CreateGuest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGuest(User model)
        {
            // We ignore Age and BirthDate validation for guests since they might not enter them on the form
            ModelState.Remove("Age");
            ModelState.Remove("BirthDate");

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required for new accounts.");
            }

            if (!ModelState.IsValid)
            {
                var guests = await _context.Users.AsNoTracking()
                    .Where(u => u.Role == "Guest" || u.Role == "GuestInactive")
                    .OrderByDescending(u => u.Id).ToListAsync();
                ViewBag.CreateForm = model;
                return View("Guests", guests);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                var guests = await _context.Users.AsNoTracking()
                    .Where(u => u.Role == "Guest" || u.Role == "GuestInactive")
                    .OrderByDescending(u => u.Id).ToListAsync();
                ViewBag.CreateForm = model;
                return View("Guests", guests);
            }

            var guest = new User
            {
                Name = model.Name,
                Email = model.Email,
                Role = "Guest",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Age = 25, // Safe value for SQL Server tracking purposes
                BirthDate = new DateOnly(2000, 1, 1) // Safe value
            };

            _context.Users.Add(guest);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Guest account for \"{model.Name}\" created successfully.";
            return RedirectToAction(nameof(Guests));
        }

        // POST: /Admin/DeactivateGuest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateGuest(int id)
        {
            var guest = await _context.Users.FindAsync(id);
            if (guest != null && guest.Role == "Guest")
            {
                guest.Role = "GuestInactive";
                await _context.SaveChangesAsync();
                TempData["Success"] = $"\"{guest.Name}\" has been deactivated.";
            }
            return RedirectToAction(nameof(Guests));
        }

        // POST: /Admin/ActivateGuest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateGuest(int id)
        {
            var guest = await _context.Users.FindAsync(id);
            if (guest != null && guest.Role == "GuestInactive")
            {
                guest.Role = "Guest";
                await _context.SaveChangesAsync();
                TempData["Success"] = $"\"{guest.Name}\" has been activated.";
            }
            return RedirectToAction(nameof(Guests));
        }
    }
}
