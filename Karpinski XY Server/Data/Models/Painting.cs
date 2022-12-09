using Karpinski_XY_Server.Data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Models
{
    public class Painting : DeletableEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Dimensions { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Required]
        public bool IsAvailableToSell { get; set; }

        public string ImageUrl { get; set; }
    }
}
