using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Karpinski_XY_Server.Dtos.BaseDto;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Dtos.Painting;

namespace Karpinski_XY_Server.Services.FileServices
{
    public class ImagePathService<T> : IImagePathService<T> where T : ImageBaseDto
    {
        private readonly IWebHostEnvironment _env;
        private readonly ImageFiles _imageFiles;

        public ImagePathService(IWebHostEnvironment env, IOptions<ImageFiles> imageFiles)
        {
            _env = env;
            _imageFiles = imageFiles.Value;
        }

        public string ConstructPathForConversionTo64Base(T imageDto)
        {
            var baseUrl = GetBaseUrlFromLaunchSettings();
            var relativePath = imageDto.ImagePath.Replace(baseUrl, string.Empty);
            var directory = GetFilesPath();
            var filePath = Path.Combine(directory, relativePath);
            var fullPath = $"{Directory.GetCurrentDirectory()}\\{filePath}";

            return fullPath;
        }

        public string ConstructPathForDatabase(T imageDto)
        {
            var fileName = imageDto.FileName + ".jpg";
            var directory = GetFilesPath();
            var newPath = Path.Combine(directory.TrimStart('\\', '/'), fileName);
            return newPath;
        }


        public string GetBaseUrlFromLaunchSettings()
        {
            var launchSettingsFilePath = Path.Combine(_env.ContentRootPath, "Properties", "launchSettings.json");

            if (!File.Exists(launchSettingsFilePath))
            {
                return "launchSettings.json not found";
            }

            var launchSettings = JObject.Parse(File.ReadAllText(launchSettingsFilePath));
            var applicationUrls = launchSettings["profiles"]["Karpinski_XY_Server"]["applicationUrl"].ToString();
            var firstUrl = applicationUrls.Split(';')[0];

            return firstUrl;
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
