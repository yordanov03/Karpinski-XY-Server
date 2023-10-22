using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Dtos
{
    public class PaintingPictureDto
    {
        [Required]
        public string ImageUrl { get; set; }

        public bool IsMainPicture { get; set; }

        public Guid PaintingId { get; set; }
    }
}
