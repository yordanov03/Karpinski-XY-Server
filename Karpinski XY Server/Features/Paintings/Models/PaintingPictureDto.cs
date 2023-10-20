using Karpinski_XY_Server.Models;

namespace Karpinski_XY_Server.Features.Paintings.Models
{
    public class PaintingPictureDto
    {
        public string ImageUrl { get; set; }

        public bool IsMainPicture { get; set; }

        public Guid PaintingId { get; set; }
    }
}
