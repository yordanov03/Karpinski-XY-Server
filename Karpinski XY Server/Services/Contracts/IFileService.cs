using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Painting;
using Karpinski_XY_Server.Dtos.Painting;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IFileService
    {
        Task<Result<List<PaintingImageDto>>> UpdateImagePathsAsync(List<PaintingImageDto> images);
        Task<Result<List<PaintingImageDto>>> ConvertImagePathsToBase64Async(List<PaintingImageDto> imageDtos);
        void MarkDeletedImagesAsDeleted(List<PaintingImageDto> imageDtos, List<PaintingImage> images);
    }
}
