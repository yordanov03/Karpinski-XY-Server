namespace Karpinski_XY_Server.Data.Models.Base
{
    public class DeletableEntity : Entity, IDeletableEntity
    {
        public bool IsDeleted { get; set; }
    }
}
