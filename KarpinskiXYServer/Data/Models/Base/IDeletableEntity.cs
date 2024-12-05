namespace Karpinski_XY_Server.Data.Models.Base
{
    public interface IDeletableEntity : IEntity
    {
        bool IsDeleted { get; set; }
    }
}
