using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Dtos
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsMainImage { get; set; }

        public string? File { get; set; }

        public Guid PaintingId { get; set; }
    }
}
