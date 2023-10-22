using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Dtos.Identity
{
    public class RegisterRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
