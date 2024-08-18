using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos.Painting;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IPaintingsService
    {
        public Task<Result<IEnumerable<PaintingDto>>> GetAllPaintingsToSell();
        public Task<Result<IEnumerable<PaintingDto>>> GetAvailablePaintings();
        public Task<Result<IEnumerable<PaintingDto>>> GetPaintingsOnFocus();
        public Task<Result<IEnumerable<PaintingDto>>> GetPortfolioPaintings();
        public Task<Result<PaintingDto>> GetPaintingById(Guid id);
        public Task<Result<PaintingDto>> GetPaintingToEdit(Guid id);
        public Task<Result<Guid>> Create(PaintingDto model);
        public Task<Result<PaintingDto>> Update(PaintingDto model);
        public Task<Result<bool>> Delete(Guid id);
    }
}
