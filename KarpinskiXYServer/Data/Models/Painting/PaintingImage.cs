using Karpinski_XY_Server.Data.Models.Base;

namespace Karpinski_XY_Server.Data.Models.Painting
{
    public class PaintingImage : ImageBase
    {
        public virtual Painting Painting { get; set; }
    }
}
