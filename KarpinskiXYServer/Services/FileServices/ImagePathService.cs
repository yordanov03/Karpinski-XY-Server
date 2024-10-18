using Microsoft.Extensions.Options;
using Karpinski_XY_Server.Dtos.BaseDto;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Dtos.Painting;

namespace Karpinski_XY_Server.Services.FileServices
{
    public class ImagePathService<T> : IImagePathService<T> where T : ImageBaseDto
    {
        private readonly ImageFiles _imageFiles;

        public ImagePathService(IOptions<ImageFiles> imageFiles)
        {
            _imageFiles = imageFiles.Value;
        }

        public string ConstructPathForConversionTo64Base(T imageDto)
        {
            var relativePath = imageDto.ImagePath;
            var directory = GetFilesPath();
            var filePath = Path.Combine(directory, relativePath);
            var fullPath = $"{Directory.GetCurrentDirectory()}\\{filePath}";

            return fullPath;
        }

        public string ConstructPathForDatabase(T imageDto)
        {
            var fileName = imageDto.FileName;
            var directory = GetFilesPath();
            var newPath = Path.Combine(directory, fileName);
            return newPath;
        }

        protected string GetFilesPath()
        {
            if (typeof(T) == typeof(PaintingImageDto))
            {
                return _imageFiles.PaintingFilesPath;
            }
            else if (typeof(T) == typeof(ExhibitionImageDto))
            {
                return _imageFiles.ExhibitionFilesPath;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
