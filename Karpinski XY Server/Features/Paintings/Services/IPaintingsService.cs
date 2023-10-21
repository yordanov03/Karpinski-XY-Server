using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Infrastructure.Services;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public interface IPaintingsService
    {
        public Task<IEnumerable<PaintingDto>> GetAllPaintings();
        public Task<IEnumerable<PaintingDto>> GetAvailablePaintings();
        public Task<IEnumerable<PaintingDto>> GetPaintingsOnFocus();
        public Task<IEnumerable<PaintingDto>> GetPortfolioPaintings();
        public Task<PaintingDto> GetPaintingById(Guid id);
        public Task<Result> Create(PaintingDto model);
        public Task<Result> Update(PaintingDto model);
        public Task<Result> Delete(Guid id);
    }
}
