using Barral_ELNET1_MVC.Data;
using Barral_ELNET1_MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barral_ELNET1_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard/Index
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index(string? searchDesc, DateTime? filterDate)
        {
            var today = DateTime.Today;

            // Overall summary cards
            var totalTransactions = await _context.Transactions.CountAsync();
            var totalAmount = await _context.Transactions.SumAsync(t => (decimal?)t.Amount) ?? 0m;
            var todayTransactions = await _context.Transactions
                .CountAsync(t => t.Date.Date == today);

            // Recent transactions (up to 5) with optional filter/search
            var recentQuery = _context.Transactions
                .AsNoTracking()
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDesc))
            {
                recentQuery = recentQuery.Where(t => t.Description.Contains(searchDesc));
            }

            if (filterDate.HasValue)
            {
                var selectedDate = filterDate.Value.Date;
                recentQuery = recentQuery.Where(t => t.Date.Date == selectedDate);
            }

            var recentTransactions = await recentQuery.Take(5).ToListAsync();

            // Chart data: last 7 days
            var startDate = today.AddDays(-6);

            var groupedData = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.Date.Date >= startDate)
                .GroupBy(t => t.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var groupedLookup = groupedData.ToDictionary(x => x.Date, x => x);

            var labels = new List<string>();
            var counts = new List<int>();
            var amounts = new List<decimal>();

            for (var date = startDate; date <= today; date = date.AddDays(1))
            {
                labels.Add(date.ToString("MMM dd"));

                if (groupedLookup.TryGetValue(date.Date, out var value))
                {
                    counts.Add(value.Count);
                    amounts.Add(value.Amount);
                }
                else
                {
                    counts.Add(0);
                    amounts.Add(0m);
                }
            }

            var model = new DashboardViewModel
            {
                TotalTransactions = totalTransactions,
                TotalAmount = totalAmount,
                TodayTransactions = todayTransactions,
                LastUpdated = DateTime.Now,
                SearchDesc = searchDesc,
                FilterDate = filterDate,
                RecentTransactions = recentTransactions,
                ChartLabels = labels,
                TransactionsPerDay = counts,
                AmountPerDay = amounts
            };

            return View("Dashboard", model);
        }
    }
}