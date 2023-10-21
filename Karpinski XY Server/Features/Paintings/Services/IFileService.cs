using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Infrastructure.Services;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public interface IFileService
    {
        Task<Result<List<PaintingPictureDto>>> UpdateImagePathsAsync(List<PaintingPictureDto> paintingPictures);
    }
}
