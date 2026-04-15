using Barral_ELNET1_MVC.Data;
using Barral_ELNET1_MVC.Models;
using Barral_ELNET1_MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Barral_ELNET1_MVC.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }
        // ── GET /Transaction/Index ─────────────────────────────────────────────
        [Authorize(Roles = "Admin,Guest")]
        public async Task<IActionResult> Index(string? searchDesc, string? filterDate)
        {
            var query = _context.Transactions
                .Include(t => t.Student)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDesc))
                query = query.Where(t => t.Description.Contains(searchDesc));

            if (!string.IsNullOrWhiteSpace(filterDate) &&
                DateTime.TryParse(filterDate, out var parsedDate))
                query = query.Where(t => t.Date.Date == parsedDate.Date);

            ViewBag.SearchDesc = searchDesc;
            ViewBag.FilterDate = filterDate;

            return View(await query.OrderByDescending(t => t.Date).ToListAsync());
        }

        // ── GET /Transaction/Create ────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Students = new SelectList(_context.Students.OrderBy(s => s.Name), "Id", "Name");
            return View(new Transaction { Date = DateTime.Today });
        }

        // ── POST /Transaction/Create ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students = new SelectList(_context.Students.OrderBy(s => s.Name), "Id", "Name");
                return View(transaction);
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Transaction recorded successfully.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}