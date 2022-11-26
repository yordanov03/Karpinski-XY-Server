using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY.Features.Identity.Models
{
    public class LoginRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
