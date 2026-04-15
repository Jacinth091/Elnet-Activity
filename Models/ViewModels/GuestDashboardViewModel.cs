using Barral_ELNET1_MVC.Models;

namespace Barral_ELNET1_MVC.ViewModels
{
    public class GuestDashboardViewModel
    {
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public DateTime? LatestTransactionDate { get; set; }
        public List<Transaction> RecentTransactions { get; set; } = new();
    }
}
