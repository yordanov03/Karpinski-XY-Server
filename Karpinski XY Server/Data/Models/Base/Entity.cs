namespace Karpinski_XY_Server.Data.Models.Base
{
    public abstract class Entity : IEntity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}
