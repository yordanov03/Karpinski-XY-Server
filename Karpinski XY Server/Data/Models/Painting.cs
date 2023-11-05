using Karpinski_XY_Server.Data.Models;
using Karpinski_XY_Server.Data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Models
{
    public class Painting : DeletableEntity
    {
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

        public bool IsAvailableToSell { get; set; }

        public string ShortDescription { get; set; }

        public string Technique { get; set; }

        public int Year { get; set; }

        public bool IsOnFocus { get; set; }

        public List<Image> Images { get; set; } = new List<Image>();


    }
}
