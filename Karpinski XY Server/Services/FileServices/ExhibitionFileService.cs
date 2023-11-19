using Karpinski_XY_Server.Dtos.Exhibition;


namespace Karpinski_XY_Server.Services.FileServices
{
    public class ExhibitionFileService : FileService<ExhibitionImageDto>
    {
        public ExhibitionFileService(ILogger<FileService<ExhibitionImageDto>> logger,
                                IImagePathService<ExhibitionImageDto> imagePathService)
            : base(logger, imagePathService) {}
    }
}
