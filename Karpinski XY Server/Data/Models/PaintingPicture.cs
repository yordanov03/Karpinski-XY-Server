using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Data.Models
{
    public class PaintingPicture : DeletableEntity
    {
        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public bool IsMainPicture { get; set; }

        public Guid PaintingId { get; set; }

        public virtual Painting Painting { get; set; }
    }
}
