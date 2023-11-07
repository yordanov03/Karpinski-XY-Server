using Karpinski_XY_Server.Data.Models;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IFileService
    {
        Task<Result<List<ImageDto>>> UpdateImagePathsAsync(List<ImageDto> images);
        Task<Result<List<ImageDto>>> ConvertImagePathsToBase64Async(List<ImageDto> imageDtos);
        void MarkDeletedImagesAsDeleted(List<ImageDto> imageDtos, List<Image> images);
    }
}
