using Karpinski_XY_Server.Data.Models;

namespace Karpinski_XY_Server.Dtos
{
    public class PaintingDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Dimensions { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsAvailableToSell { get; set; }

        public string ShortDescription { get; set; }

        public string Technique { get; set; }

        public int Year { get; set; }

        public bool IsOnFocus { get; set; }
        public List<ImageDto> Images{ get; set; } = new List<ImageDto>();
    }
}
