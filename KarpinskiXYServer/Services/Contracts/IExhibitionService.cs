using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos.Exhibition;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IExhibitionService
    {
        Task<Result<IEnumerable<ExhibitionDto>>> GetAllExhibitionsAsync();
        Task<Result<ExhibitionDto>> GetExhibitionByIdAsync(Guid id);
        Task<Result<ExhibitionDto>> GetExhibitionToUpdateAsync(Guid id);
        Task<Result<Guid>> CreateExhibitionAsync(ExhibitionDto model);
        Task<Result<ExhibitionDto>> UpdateExhibitionAsync(ExhibitionDto model);
        Task<Result<bool>> DeleteExhibitionAsync(Guid id);
    }
}

