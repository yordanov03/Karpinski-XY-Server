using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Features.Paintings.Models
{
    public class PaintingPictureDto
    {
        [Required]
        public string ImageUrl { get; set; }

        public bool IsMainPicture { get; set; }

        public Guid PaintingId { get; set; }
    }
}
