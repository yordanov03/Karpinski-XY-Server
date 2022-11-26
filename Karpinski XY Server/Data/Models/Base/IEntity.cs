namespace Karpinski_XY_Server.Data.Models.Base
{
    public interface IEntity
    {
        DateTime CreatedOn { get; set; }
        DateTime? ModifiedOn { get; set; }
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
    }
}
