using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barral_ELNET1_MVC.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Course selection is required.")]
        public string Course { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(18, 60, ErrorMessage = "Age must be between 18 and 60.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Birth Date is required.")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }
    }
}
