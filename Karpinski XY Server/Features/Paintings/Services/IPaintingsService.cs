using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Infrastructure.Services;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public interface IPaintingsService
    {
        public Task<Result<IEnumerable<PaintingDto>>> GetAllPaintings();
        public Task<Result<IEnumerable<PaintingDto>>> GetAvailablePaintings();
        public Task<Result<IEnumerable<PaintingDto>>> GetPaintingsOnFocus();
        public Task<Result<IEnumerable<PaintingDto>>> GetPortfolioPaintings();
        public Task<Result<PaintingDto>> GetPaintingById(Guid id);
        public Task<Result<Guid>> Create(PaintingDto model);
        public Task<Result<PaintingDto>> Update(PaintingDto model);
        public Task<Result<bool>> Delete(Guid id);
    }
}
