using Karpinski_XY_Server.Data.Models;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Karpinski_XY_Server.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly ImageFiles _imageFiles;
        private readonly IWebHostEnvironment _env;


        public FileService(ILogger<FileService> logger,
            IOptions<ImageFiles> imageFiles,
            IWebHostEnvironment env)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageFiles = imageFiles.Value;
            _env = env;
        }

        /// <summary>
        /// String to image path
        /// </summary>
        /// <param name="imageDtos"></param>
        /// <returns></returns>
        public async Task<Result<List<ImageDto>>> UpdateImagePathsAsync(List<ImageDto> imageDtos)
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
                return Result<List<ImageDto>>.Fail(new List<string> { $"Validation failed:{Environment.NewLine}{aggregatedErrors}" });
            }

            return Result<List<ImageDto>>.Success(imageDtos);
        }

        private async Task<string> UpdateImagePathAsync(ImageDto imageDto)
        {
            try
            {
                imageDto.Id = Guid.NewGuid();
                var fileName = imageDto.Id + ".jpg";
                var newPath = Path.Combine(_imageFiles.Path.TrimStart('\\', '/'), fileName);


                var imageBytes = Convert.FromBase64String(imageDto.File);
                await File.WriteAllBytesAsync(newPath, imageBytes);

                imageDto.File = null;  // Clear the Base64 string as it's no longer needed
                imageDto.ImageUrl = $"{GetBaseUrlFromLaunchSettings()}{_imageFiles.Path}\\{fileName}";  // Set the URL to the path where the file is stored
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
        public async Task<Result<List<ImageDto>>> ConvertImagePathsToBase64Async(List<ImageDto> imageDtos)
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
                return Result<List<ImageDto>>.Fail(new List<string> { $"Validation failed:{Environment.NewLine}{aggregatedErrors}" });
            }

            return Result<List<ImageDto>>.Success(imageDtos);
        }

        private async Task<string> ConvertImagePathToBase64Async(ImageDto imageDto)
        {
            //try
            //{
            //    var filePath = Path.Combine(_imageFiles.Path, imageDto.ImageUrl);
            //    var imageBytes = await File.ReadAllBytesAsync(filePath);
            //    imageDto.File = Convert.ToBase64String(imageBytes); // Set the Base64 string

            //    _logger.LogInformation("Successfully converted image path to Base64 for image: {FileName}", imageDto.ImageUrl);

            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    var errorMessage = $"Failed to convert image path to Base64 for {imageDto.ImageUrl}: {ex.Message}";
            //    _logger.LogError(ex, "Error while converting image path to Base64 for image: {FileName}", imageDto.ImageUrl);
            //    return errorMessage;
            //}
            try
            {
                var baseUrl = GetBaseUrlFromLaunchSettings();
                var relativePath = imageDto.ImageUrl.Replace(baseUrl, string.Empty);

                var filePath = Path.Combine(_imageFiles.Path, relativePath);
                var fullPath = $"{Directory.GetCurrentDirectory()}{filePath}";
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

        public void MarkDeletedImagesAsDeleted(List<ImageDto> imageDtos, List<Image> images)
        {
            var imageDtoIds = new HashSet<Guid>(imageDtos.Select(dto => dto.Id));

            // Iterate over the images and set IsDeleted to true if the Id is not in the HashSet.
            images.ForEach(image => {
                if (!imageDtoIds.Contains(image.Id))
                {
                    image.IsDeleted = true;
                }
            });

            // Since the method is void, there is no return statement needed.
        }
    }
}
