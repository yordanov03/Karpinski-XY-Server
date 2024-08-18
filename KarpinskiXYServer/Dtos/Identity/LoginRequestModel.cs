using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Dtos.Identity
{
    public class LoginRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
