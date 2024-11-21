using System.ComponentModel.DataAnnotations;

namespace PROG6212_New_POE.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public int HoursWorked { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        public string Notes { get; set; }

        public string FileName { get; set; }

        public string Status { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
