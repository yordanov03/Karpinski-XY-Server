using FluentValidation;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Configuration;
using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.Extensions.Options;

namespace Karpinski_XY_Server.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly PaintingFiles _paintingFiles;
        private readonly IValidator<PaintingPictureDto> _paintingPictureDtoValidator;

        public FileService(ILogger<FileService> logger,
            IOptions<PaintingFiles> paintingFiles,
            IValidator<PaintingPictureDto> paintingPictureDtoValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paintingFiles = paintingFiles.Value;
            _paintingPictureDtoValidator = paintingPictureDtoValidator;
        }

        public Task<Result<List<PaintingPictureDto>>> UpdateImagePathsAsync(List<PaintingPictureDto> paintingPictures)
        {
            _logger.LogInformation("Starting to update image paths for {Count} painting(s).", paintingPictures.Count);

            var errors = new List<string>();

            foreach (var paintingPicture in paintingPictures)
            {
                var error = UpdateImagePath(paintingPicture);
                if (error != null)
                {
                    errors.Add(error);
                }
            }

            _logger.LogInformation("Completed updating image paths for {Count} painting(s).", paintingPictures.Count);

            if (errors.Count > 0)
            {
                var aggregatedErrors = string.Join(Environment.NewLine, errors);
                return Task.FromResult(Result<List<PaintingPictureDto>>.Fail(new List<string> { $"Validation failed:{Environment.NewLine}{aggregatedErrors}" }));
            }

            return Task.FromResult(Result<List<PaintingPictureDto>>.Success(paintingPictures));
        }

        private string UpdateImagePath(PaintingPictureDto paintingPicture)
        {
            var validationResult = _paintingPictureDtoValidator.Validate(paintingPicture);
            if (!validationResult.IsValid)
            {
                var aggregatedErrors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
                return $"Validation failed:{Environment.NewLine}{aggregatedErrors}";
            }

            try
            {
                var fileName = Path.GetFileName(paintingPicture.ImageUrl);
                var newPath = Path.Combine(_paintingFiles.Path, fileName);
                File.Copy(paintingPicture.ImageUrl, newPath);

                paintingPicture.ImageUrl = newPath;
                _logger.LogInformation("Successfully updated image path for painting: {FileName}", fileName);

                return null;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to update image path for {paintingPicture.ImageUrl}: {ex.Message}";
                _logger.LogError(ex, "Error while updating image path for painting: {FileName}", paintingPicture.ImageUrl);
                return errorMessage;
            }
        }

    }
}
