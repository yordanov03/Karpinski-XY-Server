using Karpinski_XY_Server.Features.Paintings.Models;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public interface IFileService
    {
        Task<List<PaintingPictureDto>> UpdateImagePathsAsync(List<PaintingPictureDto> paintingPictures);
    }
}
