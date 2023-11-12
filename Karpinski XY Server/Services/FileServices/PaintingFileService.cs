using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Dtos.Painting;
using Microsoft.Extensions.Options;

namespace Karpinski_XY_Server.Services.FileServices
{
    public class PaintingFileService : FileService<PaintingImageDto>
    {
        public PaintingFileService(ILogger<FileService<PaintingImageDto>> logger,
                                 IOptions<ImageFiles> imageFiles,
                                 IWebHostEnvironment env)
            : base(logger, imageFiles, env) { }
    }
}
