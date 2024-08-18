using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos.Exhibition;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IExhibitionService
    {
        Task<Result<IEnumerable<ExhibitionDto>>> GetAllExhibitions();
        Task<Result<ExhibitionDto>> GetExhibitionById(Guid id);
        Task<Result<ExhibitionDto>> GetExhibitionToUpdate(Guid id);
        Task<Result<Guid>> CreateExhibition(ExhibitionDto model);
        Task<Result<ExhibitionDto>> UpdateExhibition(ExhibitionDto model);
        Task<Result<bool>> DeleteExhibition(Guid id);
    }
}

