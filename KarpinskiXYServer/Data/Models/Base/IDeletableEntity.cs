﻿namespace Karpinski_XY_Server.Data.Models.Base
{
    public interface IDeletableEntity : IEntity
    {
        DateTime? DeletedOn { get; set; }
        bool IsDeleted { get; set; }
    }
}
