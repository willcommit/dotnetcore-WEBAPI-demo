using System.ComponentModel.DataAnnotations;

namespace DatingApp_API.DTOs
{
    public class UserForLoginDTO
    {
        // API will check User and PWD with DB so validation is not needed.
        public string Username { get; set; }
        public string Password { get; set; }
    }
}