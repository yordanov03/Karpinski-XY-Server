using Karpinski_XY_Server.Data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Karpinski_XY_Server.Data.Models.Exhibition
{
    public class Exhibition : DeletableEntity
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public string Organizer { get; set; }

        [Required]
        public string Location { get; set; }

        public string Link { get; set; }

        public List<ExhibitionImage> ExhibitionImages { get; set; } = new List<ExhibitionImage>();
    }
}
