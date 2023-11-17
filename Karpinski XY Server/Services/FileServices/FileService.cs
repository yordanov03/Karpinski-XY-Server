using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos.BaseDto;
using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Dtos.Painting;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Karpinski_XY_Server.Services.FileServices
{
    public abstract class FileService<T> : IFileService<T> where T : ImageBaseDto
    {
        private readonly ILogger<FileService<T>> _logger;
        private readonly ImageFiles _imageFiles;
        private readonly IWebHostEnvironment _env;


        public FileService(ILogger<FileService<T>> logger,
            IOptions<ImageFiles> imageFiles,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _imageFiles = imageFiles.Value;
            _env = env;
        }

        /// <summary>
        /// String to image path
        /// </summary>
        /// <param name="imageDtos"></param>
        /// <returns></returns>
        public async Task<Result<List<T>>> UpdateImagePathsAsync(List<T> imageDtos)
        {
            _logger.LogInformation("Starting to update image paths for {Count} image(s).", imageDtos.Count);

            var errors = new List<string>();

            foreach (var imageDto in imageDtos)
            {
                var error = await UpdateImagePathAsync(imageDto);
                if (error != null)
                {
                    errors.Add(error);
                }
            }

            _logger.LogInformation("Completed updating image paths for {Count} image(s).", imageDtos.Count);

            if (errors.Count > 0)
            {
                var aggregatedErrors = string.Join(Environment.NewLine, errors);
                return Result<List<T>>.Fail(new List<string> { $"Validation failed:{Environment.NewLine}{aggregatedErrors}" });
            }

            return Result<List<T>>.Success(imageDtos);
        }

        private async Task<string> UpdateImagePathAsync(T imageDto)
        {
            try
            {
                //imageDto.Id = Guid.NewGuid();
                //var fileName = imageDto.Id + ".jpg";
                //var newPath = Path.Combine(_imageFiles.PaintingFilesPath.TrimStart('\\', '/'), fileName);
                imageDto.Id = Guid.NewGuid();
                var fileName = imageDto.Id + ".jpg";
                var newPath = ConstructPathForDatabase(imageDto);


                var imageBytes = Convert.FromBase64String(imageDto.File);
                await File.WriteAllBytesAsync(newPath, imageBytes);

                imageDto.File = null;  // Clear the Base64 string as it's no longer needed
                imageDto.ImageUrl = $"{GetBaseUrlFromLaunchSettings()}\\{newPath}";  // Set the URL to the path where the file is stored
                _logger.LogInformation("Successfully updated image path for image: {FileName}", fileName);

                return null;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to update image path for {imageDto.ImageUrl}: {ex.Message}";
                _logger.LogError(ex, "Error while updating image path for image: {FileName}", imageDto.ImageUrl);
                return errorMessage;
            }
        }

        // Image to string
        public async Task<Result<List<T>>> ConvertImagePathsToBase64Async(List<T> imageDtos)
        {
            _logger.LogInformation("Starting to convert image paths to Base64 for {Count} image(s).", imageDtos.Count);

            var errors = new List<string>();

            foreach (var imageDto in imageDtos)
            {
                var error = await ConvertImagePathToBase64Async(imageDto);
                if (error != null)
                {
                    errors.Add(error);
                }
            }

            _logger.LogInformation("Completed converting image paths to Base64 for {Count} image(s).", imageDtos.Count);

            if (errors.Count > 0)
            {
                var aggregatedErrors = string.Join(Environment.NewLine, errors);
                return Result<List<T>>.Fail(new List<string> { $"Validation failed:{Environment.NewLine}{aggregatedErrors}" });
            }

            return Result<List<T>>.Success(imageDtos);
        }

        private async Task<string> ConvertImagePathToBase64Async(T imageDto)
        {
            try
            {
                //var baseUrl = GetBaseUrlFromLaunchSettings();
                //var relativePath = imageDto.ImageUrl.Replace(baseUrl, string.Empty);

                //var filePath = Path.Combine(_imageFiles.PaintingFilesPath, relativePath);
                //var fullPath = $"{Directory.GetCurrentDirectory()}{filePath}";
                var fullPath = ConstructPathForConversionTo64Base(imageDto);
                var imageBytes = await File.ReadAllBytesAsync(fullPath);

                imageDto.File = Convert.ToBase64String(imageBytes); // Set the Base64 string

                _logger.LogInformation("Successfully converted image path to Base64 for image: {FileName}", imageDto.ImageUrl);

                return null;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to convert image path to Base64 for {imageDto.ImageUrl}: {ex.Message}";
                _logger.LogError(ex, "Error while converting image path to Base64 for image: {FileName}", imageDto.ImageUrl);
                return errorMessage;
            }
        }

        private string GetBaseUrlFromLaunchSettings()
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

        //Other

        public void MarkDeletedImagesAsDeleted(List<T> imageDtos, List<ImageBase> images)
        {
            var imageDtoIds = new HashSet<Guid>(imageDtos.Select(dto => dto.Id));

            images.ForEach(image =>
            {
                if (!imageDtoIds.Contains(image.Id))
                {
                    image.IsDeleted = true;
                }
            });
        }


        protected string ConstructPathForConversionTo64Base(T imageDto)
        {
            //var baseUrl = GetBaseUrlFromLaunchSettings();
            //var relativePath = imageDto.ImageUrl.Replace(baseUrl, string.Empty);

            //var filePath = Path.Combine(_imageFiles.PaintingFilesPath, relativePath);
            //var fullPath = $"{Directory.GetCurrentDirectory()}{filePath}";
            var baseUrl = GetBaseUrlFromLaunchSettings();
            var relativePath = imageDto.ImageUrl.Replace(baseUrl, string.Empty);
            var directory = GetFilesPath();
            var filePath = Path.Combine(directory, relativePath);
            var fullPath = $"{Directory.GetCurrentDirectory()}\\{filePath}";

            return fullPath;
        }

        protected string ConstructPathForDatabase(T imageDto)
        {
            var fileName = imageDto.Id + ".jpg";
            var directory = GetFilesPath();
            var newPath = Path.Combine(directory.TrimStart('\\', '/'), fileName);
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
