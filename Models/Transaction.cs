using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barral_ELNET1_MVC.Models
{
    public class Transaction
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Amount must be a positive value.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }



        [Required(ErrorMessage = "Please select a student.")]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

    }
}
