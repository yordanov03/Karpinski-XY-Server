using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly PaintingFiles _paintingFiles;

        public FileService(ILogger<FileService> logger, IOptions<PaintingFiles> paintingFiles)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paintingFiles = paintingFiles.Value;
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

            var result = errors.Count == 0
            ? Result<List<PaintingPictureDto>>.Success(paintingPictures)
            : Result<List<PaintingPictureDto>>.Fail(errors);

            return Task.FromResult(result);
        }

        private string UpdateImagePath(PaintingPictureDto paintingPicture)
        {
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
