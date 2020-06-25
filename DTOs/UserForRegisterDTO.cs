using System.ComponentModel.DataAnnotations;

namespace DatingApp_API.DTOs
{
    public class UserForRegisterDTO
    {
        //added validation via attributes
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8,MinimumLength=5, ErrorMessage="The password must be between 4 and 8 char")]
        public string Password { get; set; }
    }
}