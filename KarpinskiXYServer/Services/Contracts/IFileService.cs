using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos.BaseDto;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IFileService<T> where T : ImageBaseDto
    {
        Task<Result<List<T>>> UpdateImagePathsAsync(List<T> images);
        Task<Result<List<T>>> ConvertImagePathsToBase64Async(List<T> imageDtos);
    }
}
