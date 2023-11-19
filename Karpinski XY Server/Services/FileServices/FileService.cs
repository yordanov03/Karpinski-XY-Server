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
        private readonly IImagePathService<T> _imagePathService;


        public FileService(ILogger<FileService<T>> logger,
            IOptions<ImageFiles> imageFiles,
            IWebHostEnvironment env,
            IImagePathService<T> imagePathService)
        {
            _logger = logger;
            _imageFiles = imageFiles.Value;
            _env = env;
            _imagePathService = imagePathService;
        }

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
                imageDto.Id = Guid.NewGuid();
                var fileName = imageDto.FileName + ".jpg";
                var newPath =_imagePathService.ConstructPathForDatabase(imageDto);


                var imageBytes = Convert.FromBase64String(imageDto.File);
                await File.WriteAllBytesAsync(newPath, imageBytes);

                imageDto.File = null; 
                imageDto.ImagePath = $"{_imagePathService.GetBaseUrlFromLaunchSettings()}\\{newPath}";
                _logger.LogInformation("Successfully updated image path for image: {FileName}", fileName);

                return null;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to update image path for {imageDto.ImagePath}: {ex.Message}";
                _logger.LogError(ex, "Error while updating image path for image: {FileName}", imageDto.ImagePath);
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
                var fullPath = _imagePathService.ConstructPathForConversionTo64Base(imageDto);
                var imageBytes = await File.ReadAllBytesAsync(fullPath);

                imageDto.File = Convert.ToBase64String(imageBytes); // Set the Base64 string

                _logger.LogInformation("Successfully converted image path to Base64 for image: {FileName}", imageDto.ImagePath);

                return null;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to convert image path to Base64 for {imageDto.ImagePath}: {ex.Message}";
                _logger.LogError(ex, "Error while converting image path to Base64 for image: {FileName}", imageDto.ImagePath);
                return errorMessage;
            }
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
    }
}
