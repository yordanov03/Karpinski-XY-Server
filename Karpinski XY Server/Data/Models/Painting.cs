using Karpinski_XY_Server.Data.Models.Base;

namespace Karpinski_XY_Server.Models
{
    public class Painting : DeletableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsAvailableToSell { get; set; }
    }
}
