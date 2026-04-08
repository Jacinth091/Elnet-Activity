using Barral_ELNET1_MVC.Models;

namespace Barral_ELNET1_MVC.ViewModels;

public class DashboardViewModel
{
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public int TodayTransactions { get; set; }
    public DateTime LastUpdated { get; set; }

    public string? SearchDesc { get; set; }
    public DateTime? FilterDate { get; set; }

    public List<Transaction> RecentTransactions { get; set; } = new();

    // Chart data for the last 7 days
    public List<string> ChartLabels { get; set; } = new();
    public List<int> TransactionsPerDay { get; set; } = new();
    public List<decimal> AmountPerDay { get; set; } = new();
}