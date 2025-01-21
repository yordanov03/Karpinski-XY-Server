using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos.Painting;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IPaintingsService
    {
        public Task<Result<Guid>> CreateAsync(PaintingDto model);
        public Task<Result<PaintingDto>> GetPaintingToEditAsync(Guid id);
        public Task<Result<PaintingDto>> UpdateAsync(PaintingDto model);
        public Task<Result<bool>> DeleteAsync(Guid id);
        public Task<Result<PaintingDto>> GetPaintingByIdAsync(Guid id);
        public Task<Result<IEnumerable<PaintingDto>>> GetAllPaintingsToSellAsync();
        public Task<Result<IEnumerable<PaintingDto>>> GetAvailablePaintingsAsync();
        public Task<Result<IEnumerable<PaintingDto>>> GetPaintingsOnFocusAsync();
        public Task<Result<IEnumerable<PaintingDto>>> GetPortfolioPaintingsAsync();
    }
}
