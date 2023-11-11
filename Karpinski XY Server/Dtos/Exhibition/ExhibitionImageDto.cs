namespace Karpinski_XY_Server.Dtos.Exhibition
{
    public class ExhibitionImageDto
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsMainImage { get; set; }

        public string? File { get; set; }

        public Guid EntityId { get; set; }
    }
}
