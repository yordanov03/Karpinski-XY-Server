using Karpinski_XY.Data;
using Karpinski_XY_Server.Data.Models.Painting;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY_Server.Data.Repositories
{
    public class PaintingRepository : IPaintingRepository
    {
        private readonly ApplicationDbContext _context;

        public PaintingRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Painting painting)
        {
            await _context.Paintings.AddAsync(painting);
        }

        public async Task DeleteAsync(Guid id)
        {
            var painting = await FindByIdAsync(id);
            if (painting != null)
            {
                painting.IsDeleted = true;
                painting.PaintingImages.ForEach(image => image.IsDeleted = true);
                painting.PaintingImages.ForEach(image => image.ModifiedOn = DateTime.Now);
                _context.Paintings.Update(painting);
                _context.PaintingImages.UpdateRange(painting.PaintingImages);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<Painting>> GetAllToSellAsync()
        {
            return await _context.Paintings
                .Include(p => p.PaintingImages)
                .Where(p => !p.IsDeleted && p.IsAvailableToSell)
                .ToListAsync();
        }

        public async Task<IEnumerable<Painting>> GetAvailableAsync()
        {
            return await _context.Paintings
                .Include(p => p.PaintingImages)
                .Where(p => p.IsAvailableToSell && !p.IsDeleted && !p.IsOnFocus)
                .ToListAsync();
        }

        public async Task<IEnumerable<Painting>> GetPortfolioAsync()
        {
            return await _context.Paintings
                .Include(p => p.PaintingImages.OrderBy(i => !i.IsMainImage))
                .Where(p => !p.IsAvailableToSell && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Painting>> GetOnFocusAsync()
        {
            return await _context.Paintings
                .Include(p => p.PaintingImages.Where(i => i.IsMainImage))
                .Where(p => p.IsAvailableToSell && !p.IsDeleted && p.IsOnFocus)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string name)
        {
            return await _context.Paintings
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .AnyAsync(p => p.Name == name);
        }

        public async Task<Painting> FindByIdAsync(Guid id)
        {
            return await _context.Paintings
                .Include(p => p.PaintingImages
                    .Where(i => !i.IsDeleted)
                    .OrderByDescending(i => i.IsMainImage))
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
