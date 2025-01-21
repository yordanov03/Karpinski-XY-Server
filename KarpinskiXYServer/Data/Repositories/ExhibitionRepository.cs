using Karpinski_XY.Data;
using Karpinski_XY_Server.Data.Models.Exhibition;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY_Server.Data.Repositories
{
    public class ExhibitionRepository : IExhibitionRepository
    {
        private readonly ApplicationDbContext _context;

        public ExhibitionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string title)
        {
            return await _context.Exhibitions
                .Where(i => !i.IsDeleted)
                .AnyAsync(i => i.Title == title);
        }

        public async Task<Exhibition> FindByIdAsync(Guid id)
        {
            return await _context.Exhibitions
                .Include(e => e.ExhibitionImages
                    .Where(i => !i.IsDeleted)
                    .OrderBy(i => !i.IsMainImage))
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Exhibition exhibition)
        {
            await _context.Exhibitions.AddAsync(exhibition);
        }

        public async Task Delete(Guid id)
        {
            var exhibition = await FindByIdAsync(id);
            if (exhibition != null)
            {
                exhibition.IsDeleted = true;
                exhibition.ExhibitionImages.ForEach(image => image.IsDeleted = true);
                exhibition.ExhibitionImages.ForEach(image => image.ModifiedOn = DateTime.Now);
                _context.Exhibitions.Update(exhibition);
                _context.ExhibitionImages.UpdateRange(exhibition.ExhibitionImages);
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<Exhibition>> GetAllAsync()
        {
            return await _context.Exhibitions
                .Include(e => e.ExhibitionImages)
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }
    }

}
