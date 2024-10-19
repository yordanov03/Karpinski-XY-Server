using Karpinski_XY_Server.Dtos.Painting;

namespace Karpinski_XY_Server.Services.FileServices
{
    public class PaintingFileService : FileService<PaintingImageDto>
    {
        public PaintingFileService(ILogger<FileService<PaintingImageDto>> logger, IImagePathService<PaintingImageDto> imagePathService)
            : base(logger, imagePathService) {}
    }
}
