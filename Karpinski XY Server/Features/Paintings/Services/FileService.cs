using Microsoft.Extensions.Logging;
using Karpinski_XY_Server.Features.Paintings.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        public async Task<List<PaintingPictureDto>> UpdateImagePathsAsync(List<PaintingPictureDto> paintingPictures)
        {
            _logger.LogInformation("Starting to update image paths for {Count} painting(s).", paintingPictures.Count);

            foreach (var paintingPicture in paintingPictures)
            {
                try
                {
                    string fileName = Path.GetFileName(paintingPicture.ImageUrl);
                    string newPath = Path.Combine("C:\\Users\\sveto\\source\\repos\\Karpinski-XY-Server\\Karpinski XY Server\\Resources\\Images", fileName);
                    File.Copy(paintingPicture.ImageUrl, newPath);

                    paintingPicture.ImageUrl = newPath;

                    _logger.LogInformation("Successfully updated image path for painting: {FileName}", fileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating image path for painting: {FileName}", paintingPicture.ImageUrl);
                    continue;
                }
            }

            _logger.LogInformation("Completed updating image paths for {Count} painting(s).", paintingPictures.Count);

            return paintingPictures;
        }
    }
}
