using AutoMapper;
using Karpinski_XY.Data;
using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Infrastructure.Services;
using Karpinski_XY_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY_Server.Features.Paintings.Services
{
    public class PaintingsService : IPaintingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<PaintingsService> _logger;

        public PaintingsService(ApplicationDbContext context,
            IMapper mapper,
            IFileService fileService,
            ILogger<PaintingsService> logger)
        {
            _context = context;
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;   
        }

        public async Task<Result<Guid>> Create(PaintingDto model)
        {
            _logger.LogInformation("Creating a new painting");

            model.Id = Guid.NewGuid();
            var updateResult = await _fileService.UpdateImagePathsAsync(model.PaintingPictures);

            if (!updateResult.Succeeded)
            {
                return Result<Guid>.Fail("Failed to update image paths.");
            }

            model.PaintingPictures = updateResult.Value;

            var painting = _mapper.Map<Painting>(model);
            _context.Add(painting);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully created a new painting");
            return Result<Guid>.Success(model.Id);
        }


        public async Task<Result<IEnumerable<PaintingDto>>> GetAllPaintings()
        {
            _logger.LogInformation("Fetching all paintings");

            var paintings = await _context
                .Paintings
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            return Result<IEnumerable<PaintingDto>>.Success(_mapper.Map<IEnumerable<PaintingDto>>(paintings));
        }

        public async Task<Result<IEnumerable<PaintingDto>>> GetAvailablePaintings()
        {
            _logger.LogInformation("Fetching all available paintings");

            var paintings = await _context
                .Paintings
                .Where(p => p.IsAvailableToSell && !p.IsDeleted)
                .ToListAsync();

            return Result<IEnumerable<PaintingDto>>.Success(_mapper.Map<IEnumerable<PaintingDto>>(paintings));
        }

        public async Task<Result<PaintingDto>> GetPaintingById(Guid id)
        {
            _logger.LogInformation($"Fetching painting with id {id}");

            var painting = FindPaintingById(id);
            if (painting == null)
            {
                return Result<PaintingDto>.Fail($"Painting with ID {id} not found.");
            }

            return Result<PaintingDto>.Success(_mapper.Map<PaintingDto>(painting));
        }

        public async Task<Result<IEnumerable<PaintingDto>>> GetPortfolioPaintings()
        {
            _logger.LogInformation("Fetching portfolio paintings");

            var paintings = await _context
                .Paintings
                .Where(p => !p.IsAvailableToSell)
                .ToListAsync();

            return Result<IEnumerable<PaintingDto>>.Success(_mapper.Map<IEnumerable<PaintingDto>>(paintings));
        }

        public async Task<Result<PaintingDto>> Update(PaintingDto model)
        {
            _logger.LogInformation($"Looking to update painting with id {model.Id}");

            var painting = FindPaintingById(model.Id);
            if (painting == null)
            {
                _logger.LogWarning($"Painting with ID {model.Id} not found.");
                return Result<PaintingDto>.Fail("Painting not found.");
            }

            _mapper.Map(model, painting);
            _context.Update(painting);
            await _context.SaveChangesAsync();

            var updatedModel = _mapper.Map<PaintingDto>(painting);
            _logger.LogInformation($"Updated painting successfully");
            return Result<PaintingDto>.Success(updatedModel);
        }

        public async Task<Result<bool>> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting painting with {id}");

            var painting = FindPaintingById(id);
            if (painting == null)
            {
                _logger.LogWarning($"Painting with ID {id} not found.");
                return Result<bool>.Fail($"Painting with ID {id} not found.");
            }

            painting.IsDeleted = true;
            _context.Update(painting);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted painting successfully");
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<PaintingDto>>> GetPaintingsOnFocus()
        {
            _logger.LogInformation($"Getting paintings on focus");

            var paintings = await _context
                .Paintings
                .Where(p => p.IsAvailableToSell && p.IsDeleted == false && p.OnFocus == true)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();

            var mapped = _mapper.Map<List<Painting>, IEnumerable<PaintingDto>>(paintings);
            return Result<IEnumerable<PaintingDto>>.Success(mapped);
        }



        private Painting FindPaintingById(Guid id)
        => _context
            .Paintings
            .Where(p => p.Id == id)
            .FirstOrDefault();


    }
}
