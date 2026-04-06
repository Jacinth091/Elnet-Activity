using Microsoft.EntityFrameworkCore;
using Barral_ELNET1_MVC.Models;
namespace Barral_ELNET1_MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
    }
}
