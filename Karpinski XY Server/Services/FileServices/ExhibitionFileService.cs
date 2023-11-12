using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos.Exhibition;
using Microsoft.Extensions.Options;

namespace Karpinski_XY_Server.Services.FileServices
{
    public class ExhibitionFileService : FileService<ExhibitionImageDto>
    {
        public ExhibitionFileService(ILogger<FileService<ExhibitionImageDto>> logger,
                                IOptions<ImageFiles> imageFiles,
                                IWebHostEnvironment env)
       : base(logger, imageFiles, env)
        {
        }
    }
}
