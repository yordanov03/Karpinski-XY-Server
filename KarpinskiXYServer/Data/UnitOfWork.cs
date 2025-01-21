using Karpinski_XY.Data;
using Karpinski_XY_Server.Data.Repositories;

namespace Karpinski_XY_Server.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IPaintingRepository Paintings { get; }
        public IExhibitionRepository Exhibitions { get; }

        public UnitOfWork(ApplicationDbContext context,
                          IPaintingRepository paintingRepository,
                          IExhibitionRepository exhibitionRepository)
        {
            _context = context;
            Paintings = paintingRepository;
            Exhibitions = exhibitionRepository;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
