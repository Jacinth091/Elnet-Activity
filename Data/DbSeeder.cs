using Barral_ELNET1_MVC.Data;
using Barral_ELNET1_MVC.Models;
using BCrypt.Net;

using Microsoft.EntityFrameworkCore;
namespace Barral_ELNET1_MVC.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context, ILogger? logger = null)
    {
        // Apply pending migrations first (database source of truth)
        context.Database.Migrate();

        SeedUsers(context, logger);
        SeedStudents(context, logger);
        SeedTransactions(context, logger);
    }

    private static void SeedUsers(AppDbContext context, ILogger? logger)
    {
        // Avoid duplicates by unique email, not by table count only.
        if (!context.Users.Any(u => u.Email == "admin@example.com"))
        {
            context.Users.Add(new User
            {
                Name = "System Administrator",
                Age = 30,
                Email = "admin@example.com",
                BirthDate = DateOnly.FromDateTime(new DateTime(1996, 1, 1)),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                Role = "Admin"
            });

            context.SaveChanges();
            logger?.LogInformation("Seeded default admin user.");
        }
    }

    private static void SeedStudents(AppDbContext context, ILogger? logger)
    {
        if (context.Students.Any())
        {
            return;
        }

        // Keep values realistic and aligned with model constraints.
        var students = new List<Student>
        {
            new() { Name = "Alyssa Cruz", Course = "BSIT", Age = 20, Email = "alyssa.cruz@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2005, 2, 10)) },
            new() { Name = "Marco Reyes", Course = "BSCS", Age = 22, Email = "marco.reyes@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2003, 7, 4)) },
            new() { Name = "Janelle Santos", Course = "BSIS", Age = 19, Email = "janelle.santos@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2006, 1, 25)) },
            new() { Name = "Daniel Flores", Course = "BSIT", Age = 21, Email = "daniel.flores@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2004, 11, 9)) },
            new() { Name = "Patricia Gomez", Course = "BSA", Age = 23, Email = "patricia.gomez@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2002, 5, 15)) },
            new() { Name = "Kevin Ramos", Course = "BSEd", Age = 24, Email = "kevin.ramos@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2001, 12, 1)) },
            new() { Name = "Angela Mendoza", Course = "BSN", Age = 20, Email = "angela.mendoza@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2005, 8, 20)) },
            new() { Name = "John Velasco", Course = "BSCS", Age = 18, Email = "john.velasco@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2007, 3, 18)) },
            new() { Name = "Christine Lim", Course = "BSBA", Age = 21, Email = "christine.lim@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2004, 9, 30)) },
            new() { Name = "Nathan Aquino", Course = "BSIT", Age = 22, Email = "nathan.aquino@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2003, 6, 6)) }
        };

        context.Students.AddRange(students);
        context.SaveChanges();

        logger?.LogInformation("Seeded {Count} students.", students.Count);
    }

    private static void SeedTransactions(AppDbContext context, ILogger? logger)
    {
        if (context.Transactions.Any())
        {
            return;
        }

        var students = context.Students
            .OrderBy(s => s.Id)
            .ToList();

        if (!students.Any())
        {
            logger?.LogWarning("No students available. Skipped transaction seeding to avoid invalid foreign keys.");
            return;
        }

        var descriptions = new[]
        {
            "Tuition Fee - 1st Semester",
            "Laboratory Fee",
            "Library Fee",
            "Registration Fee",
            "Miscellaneous Fee",
            "Assessment Payment",
            "Computer Lab Usage",
            "ID Processing Fee"
        };

        var random = new Random(2026);
        var transactions = new List<Transaction>();
        var startDate = DateTime.Today.AddDays(-30);

        // Seed enough records for dashboard and filter testing.
        for (var i = 0; i < 18; i++)
        {
            var student = students[random.Next(students.Count)];
            var description = descriptions[random.Next(descriptions.Length)];
            var amount = Math.Round((decimal)(random.Next(500, 5000) + random.NextDouble()), 2);

            transactions.Add(new Transaction
            {
                Date = startDate.AddDays(random.Next(0, 31)),
                Description = description,
                Amount = amount,
                StudentId = student.Id
            });
        }

        context.Transactions.AddRange(transactions);
        context.SaveChanges();

        logger?.LogInformation("Seeded {Count} transactions.", transactions.Count);
    }
}