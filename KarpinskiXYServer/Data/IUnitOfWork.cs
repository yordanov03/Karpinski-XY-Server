using Karpinski_XY_Server.Data.Repositories;

namespace Karpinski_XY_Server.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IPaintingRepository Paintings { get; }
        IExhibitionRepository Exhibitions { get; }
        Task<int> CommitAsync();
    }
}
