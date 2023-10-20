using AutoMapper;
using Karpinski_XY.Data;
using Karpinski_XY.Infrastructure.Services;
using Karpinski_XY_Server.Data.Models;
using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public class PaintingsService : IPaintingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public PaintingsService(ApplicationDbContext context,
            IMapper mapper,
            IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Result> Create(PaintingDto model)
        {
            model.Id = Guid.NewGuid();
            var updatedPaintingPictures = await _fileService.UpdateImagePathsAsync(model.PaintingPictures);
            model.PaintingPictures = updatedPaintingPictures;
            var painting = _mapper.Map<Painting>(model);
            _context.Add(painting);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PaintingDto>> GetAllPaintings()
        {
            var paintings = await _context
                .Paintings
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaintingDto>>(paintings);
        }

        public async Task<IEnumerable<PaintingDto>> GetAvailablePaintings()
        {
            var paintings = await _context
                .Paintings
                .Where(p => p.IsAvailableToSell && p.IsDeleted == false)
                .ToListAsync();

            var mapped = _mapper.Map<List<Painting>, IEnumerable<PaintingDto>>(paintings);
            return mapped;
        }

        public async Task<PaintingDto> GetPaintingById(Guid id)
        {
            var painting = FindPaintingById(id);
            return _mapper.Map<PaintingDto>(painting);
        }


        public async Task<IEnumerable<PaintingDto>> GetPortfolioPaintings()
        {
            var paintings = await _context
                .Paintings
                .Where(p => !p.IsAvailableToSell)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaintingDto>>(paintings);
        }

        public async Task<Result> Update(PaintingDto model)
        {
            var painting = FindPaintingById(model.Id);
            var updatedPainting = _mapper.Map<Painting>(model);

            _context.Update(updatedPainting);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Result> Delete(Guid id)
        {
            var painting = FindPaintingById(id);
            painting.IsDeleted = true;
            _context.Update(painting);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PaintingDto>> GetPaintingsOnFocus()
        {
            var paintings = await _context
             .Paintings
             .Where(p => p.IsAvailableToSell && p.IsDeleted == false && p.OnFocus == true)
             .OrderByDescending(p => p.CreatedOn)
             .ToListAsync();

            var mapped = _mapper.Map<List<Painting>, IEnumerable<PaintingDto>>(paintings);
            return mapped;
        }

        private Painting FindPaintingById(Guid id)
        => _context
            .Paintings
            .Where(p => p.Id == id)
            .FirstOrDefault();


    }
}
