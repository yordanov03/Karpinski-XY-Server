using Karpinski_XY.Infrastructure.Services;
using Karpinski_XY_Server.Features.Paintings.Models;

namespace Karpinski_XY_Server.Features.Paintings
{
    public interface IPaintingsService
    {
        public Task<IEnumerable<PaintingDto>> GetAllPaitings();
        public Task<IEnumerable<PaintingDto>> GetAvailablePaitings();
        public Task<IEnumerable<PaintingDto>> GetPortfolioPaitings();
        public Task<PaintingDto> GetPaitingById(Guid id);
        public Task<Guid> Create(PaintingDto model);
        public Task<Result> Update(PaintingDto model);
        public Task<Result> Delete(Guid id);
    }
}
