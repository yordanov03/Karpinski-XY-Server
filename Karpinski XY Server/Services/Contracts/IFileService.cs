using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Dtos;

namespace Karpinski_XY_Server.Services.Contracts
{
    public interface IFileService
    {
        Task<Result<List<PaintingPictureDto>>> UpdateImagePathsAsync(List<PaintingPictureDto> paintingPictures);
    }
}
