using System.ComponentModel.DataAnnotations;

namespace Barral_ELNET1_MVC.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Fullname is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be  between 2 and 100 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Course is required.")]
        public string Course { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "Age must be  between 18 and 100.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Birth Date is required.")]
        public DateOnly BirthDate { get; set; }

    }
}
