using System.ComponentModel.DataAnnotations;

namespace PROG6212_New_POE.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
   
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
