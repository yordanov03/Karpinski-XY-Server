using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Data.Models.Base
{
    public class ImageBase : DeletableEntity
    {
        public string ImageUrl { get; set; }

        [Required]
        public bool IsMainImage { get; set; }

        public Guid EntityId { get; set; }
    }
}
