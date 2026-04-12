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
        // Avoid duplicates by unique email
        if (!context.Users.Any(u => u.Email == "admin@example.com"))
        {
            context.Users.Add(new User
            {
                Name = "Administrator",
                Email = "admin@example.com",
                BirthDate = DateOnly.FromDateTime(new DateTime(2000, 1, 1)),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                Role = "Admin",
                Age = 26
            });

            logger?.LogInformation("Seeded default admin user.");
        }

        if (!context.Users.Any(u => u.Email == "guest@example.com"))
        {
            context.Users.Add(new User
            {
                Name = "Guest User",
                Email = "guest@example.com",
                BirthDate = DateOnly.FromDateTime(new DateTime(2002, 5, 10)),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Guest123"),
                Role = "Guest",
                Age = 24
            });

            logger?.LogInformation("Seeded default guest user.");
        }

        context.SaveChanges();
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
            new() { Name = "Alyssa Cruz", Course = "Bachelor of Science in Information Technology", Age = 20, Email = "alyssa.cruz@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2005, 2, 10)) },
            new() { Name = "Marco Reyes", Course = "Bachelor of Science in Computer Science", Age = 22, Email = "marco.reyes@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2003, 7, 4)) },
            new() { Name = "Janelle Santos", Course = "Bachelor of Science in Information Systems", Age = 19, Email = "janelle.santos@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2006, 1, 25)) },
            new() { Name = "Daniel Flores", Course = "Bachelor of Science in Information Technology", Age = 21, Email = "daniel.flores@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2004, 11, 9)) },
            new() { Name = "Patricia Gomez", Course = "Bachelor of Science in Accountancy", Age = 23, Email = "patricia.gomez@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2002, 5, 15)) },
            new() { Name = "Kevin Ramos", Course = "Bachelor of Secondary Education", Age = 24, Email = "kevin.ramos@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2001, 12, 1)) },
            new() { Name = "Angela Mendoza", Course = "Bachelor of Science in Nursing", Age = 20, Email = "angela.mendoza@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2005, 8, 20)) },
            new() { Name = "John Velasco", Course = "Bachelor of Science in Computer Science", Age = 18, Email = "john.velasco@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2007, 3, 18)) },
            new() { Name = "Christine Lim", Course = "Bachelor of Science in Business Administration", Age = 21, Email = "christine.lim@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2004, 9, 30)) },
            new() { Name = "Nathan Aquino", Course = "Bachelor of Science in Information Technology", Age = 22, Email = "nathan.aquino@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2003, 6, 6)) },
            new() { Name = "Sophia Loren", Course = "Bachelor of Science in Information Technology", Age = 20, Email = "sophia.loren@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2005, 3, 15)) },
            new() { Name = "Liam Neeson", Course = "Bachelor of Science in Computer Science", Age = 22, Email = "liam.neeson@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2003, 8, 12)) },
            new() { Name = "Emma Watson", Course = "Bachelor of Science in Information Systems", Age = 19, Email = "emma.watson@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2006, 2, 28)) },
            new() { Name = "Chris Pratt", Course = "Bachelor of Science in Information Technology", Age = 21, Email = "chris.pratt@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2004, 12, 5)) },
            new() { Name = "Gal Gadot", Course = "Bachelor of Science in Accountancy", Age = 23, Email = "gal.gadot@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2002, 6, 20)) },
            new() { Name = "Tom Holland", Course = "Bachelor of Secondary Education", Age = 24, Email = "tom.holland@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2001, 1, 10)) },
            new() { Name = "Zendaya Coleman", Course = "Bachelor of Science in Nursing", Age = 20, Email = "zendaya.coleman@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2005, 9, 25)) },
            new() { Name = "Henry Cavill", Course = "Bachelor of Science in Computer Science", Age = 18, Email = "henry.cavill@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2007, 4, 12)) },
            new() { Name = "Scarlett Johansson", Course = "Bachelor of Science in Business Administration", Age = 21, Email = "scarlett.j@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2004, 10, 5)) },
            new() { Name = "Robert Downey", Course = "Bachelor of Science in Information Technology", Age = 22, Email = "rdj@studentapp.edu", BirthDate = DateOnly.FromDateTime(new DateTime(2003, 5, 22)) }
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
        for (var i = 0; i < 30; i++)
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