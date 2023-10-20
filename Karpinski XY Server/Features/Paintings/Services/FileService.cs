using Karpinski_XY_Server.Features.Paintings.Models;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public class FileService : IFileService
    {
        public async Task<List<PaintingPictureDto>> UpdateImagePathsAsync(List<PaintingPictureDto> paintingPictures)
        {
            foreach (var paintingPicture in paintingPictures)
            {
                string fileName = Path.GetFileName(paintingPicture.ImageUrl);
                // Your logic to modify the ImageUrl, for example:
                string newPath = Path.Combine("C:\\Users\\sveto\\source\\repos\\Karpinski-XY-Server\\Karpinski XY Server\\Resources\\Images", fileName);
                File.Copy(paintingPicture.ImageUrl, newPath);  // Assuming the ImageUrl contains the source path

                paintingPicture.ImageUrl = newPath;
            }

            return paintingPictures;
        }
    }
}
