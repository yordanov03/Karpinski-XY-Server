using Karpinski_XY_Server.Data.Models.Base;

namespace Karpinski_XY_Server.Data.Models.Exhibition
{
    public class ExhibitionImage : ImageBase
    {
        public virtual Exhibition Exhibition { get; set; }
    }
}
