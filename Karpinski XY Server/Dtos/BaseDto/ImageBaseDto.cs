namespace Karpinski_XY_Server.Dtos.BaseDto
{
    public class ImageBaseDto
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsMainImage { get; set; }

        public string? File { get; set; }

        public Guid EntityId { get; set; }
    }
}
