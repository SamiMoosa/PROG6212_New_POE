using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PROG6212_New_POE.Models
{
    public class Claim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Ensures auto-increment
        public int ClaimID { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public int HoursWorked { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal HourlyRate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public string Notes { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string Status { get; set; }


        public int LecturerID { get; set; }
        public Lecturer Lecturer { get; set; }
    }
}
