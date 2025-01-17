using AutoMapper;
using FluentValidation;
using Karpinski_XY.Data;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Painting;
using Karpinski_XY_Server.Dtos.Painting;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Karpinski_XY_Server.Services
{
    public class PaintingsService : IPaintingsService
    {
        private const string AllPaintingsToSellCacheKey = "AllPaintingsToSell";
        private const string AvailablePaintingsCacheKey = "AvailablePaintings";
        private const string PortfolioPaintingsCacheKey = "PortfolioPaintings";
        private const string PaintingsOnFocusCacheKey = "PaintingsOnFocus";

        private readonly ICacheService _cacheService;
        private readonly ApplicationDbContext _context;
        private readonly IValidator<PaintingDto> _paintingValidator;
        private readonly IMapper _mapper;
        private readonly IFileService<PaintingImageDto> _fileService;
        private readonly ILogger<PaintingsService> _logger;

        public PaintingsService(
            ICacheService cacheService,
            ApplicationDbContext context,
            IValidator<PaintingDto> paintingValidator,
            IMapper mapper,
            IFileService<PaintingImageDto> fileService,
            ILogger<PaintingsService> logger)
        {
            _cacheService = cacheService;
            _context = context;
            _paintingValidator = paintingValidator;
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<Result<Guid>> Create(PaintingDto model)
        {
            if (await PaintingExists(model.Name))
            {
                return Result<Guid>.Fail("Painting with that name already exists");
            }

            var validationResult = _paintingValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogError("Validation failed for inquiry. Errors: {ValidationErrors}", string.Join(", ", errorMessages));
                return Result<Guid>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            _logger.LogInformation("Creating a new painting");

            model.Id = Guid.NewGuid();
            var updateResult = await _fileService.UpdateImagePathsAsync(model.PaintingImages);

            if (!updateResult.Succeeded)
            {
                return Result<Guid>.Fail("Failed to update image paths.");
            }

            model.PaintingImages = updateResult.Value;

            var painting = _mapper.Map<Painting>(model);
            _context.Add(painting);
            await _context.SaveChangesAsync();

            _cacheService.RemoveAll(new[]
             {
                AllPaintingsToSellCacheKey,
                AvailablePaintingsCacheKey,
                PortfolioPaintingsCacheKey,
                PaintingsOnFocusCacheKey
            });
            _logger.LogInformation("Successfully created a new painting");
            return Result<Guid>.Success(model.Id);
        }

        public async Task<Result<PaintingDto>> GetPaintingToEdit(Guid id)
        {
            _logger.LogInformation($"Fetching painting with id {id} to edit");

            var painting = await FindPaintingById(id);
            if (painting == null)
            {
                return Result<PaintingDto>.Fail($"Painting with ID {id} not found.");
            }

            var paintingDto = _mapper.Map<PaintingDto>(painting);

            // Convert image paths to Base64 strings
            var imageConversionResult = await _fileService.ConvertImagePathsToBase64Async(paintingDto.PaintingImages);
            if (!imageConversionResult.Succeeded)
            {
                return Result<PaintingDto>.Fail(imageConversionResult.Errors);
            }

            paintingDto.PaintingImages = imageConversionResult.Value;

            return Result<PaintingDto>.Success(paintingDto);
        }
        public async Task<Result<PaintingDto>> Update(PaintingDto model)
        {
            var validationResult = _paintingValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogError("Validation failed for inquiry. Errors: {ValidationErrors}", string.Join(", ", errorMessages));
                return Result<PaintingDto>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            _logger.LogInformation($"Looking to update painting with id {model.Id}");

            var painting = await FindPaintingById(model.Id);
            if (painting == null)
            {
                _logger.LogWarning($"Painting with ID {model.Id} not found.");
                return Result<PaintingDto>.Fail("Painting not found.");
            }

            painting.PaintingImages.ForEach(i => i.IsDeleted = true);
            _context.UpdateRange(painting.PaintingImages);
            painting.IsDeleted = true;
            _context.Update(painting);
            await _context.SaveChangesAsync();
            await Create(model);

            _cacheService.RemoveAll(new[]
            {
            GetPaintingByIdCacheKey(model.Id),
            AllPaintingsToSellCacheKey,
            AvailablePaintingsCacheKey,
            PortfolioPaintingsCacheKey,
            PaintingsOnFocusCacheKey
            });
            return Result<PaintingDto>.Success(model);
        }


        public async Task<Result<bool>> Delete(Guid id)
        {
            _logger.LogInformation($"Deleting painting with {id}");

            var painting = await FindPaintingById(id);
            if (painting == null)
            {
                _logger.LogWarning($"Painting with ID {id} not found.");
                return Result<bool>.Fail($"Painting with ID {id} not found.");
            }

            painting.IsDeleted = true;
            painting.PaintingImages.ForEach(image => image.IsDeleted = true);
            _context.Update(painting);
            await _context.SaveChangesAsync();

            _cacheService.RemoveAll(new[]
            {
                GetPaintingByIdCacheKey(id),
                AllPaintingsToSellCacheKey,
                AvailablePaintingsCacheKey,
                PortfolioPaintingsCacheKey,
                PaintingsOnFocusCacheKey
            });
            _logger.LogInformation($"Deleted painting successfully");
            return Result<bool>.Success(true);
        }


        public async Task<Result<IEnumerable<PaintingDto>>> GetAllPaintingsToSell()
        {
            var cachedPaintings = _cacheService.Get<IEnumerable<PaintingDto>>(AllPaintingsToSellCacheKey);

            if (cachedPaintings == null)
            {
                _logger.LogInformation("Fetching all paintings to sell from the database");

                var paintings = await _context
                    .Paintings
                    .Include(p => p.PaintingImages.Where(i => i.IsMainImage))
                    .Where(p => !p.IsDeleted && p.IsAvailableToSell)
                    .ToListAsync();

                cachedPaintings = _mapper.Map<IEnumerable<PaintingDto>>(paintings);

                // Use default cache settings
                _cacheService.Set(AllPaintingsToSellCacheKey, cachedPaintings);
            }

            return Result<IEnumerable<PaintingDto>>.Success(cachedPaintings);
        }


        public async Task<Result<IEnumerable<PaintingDto>>> GetAvailablePaintings()
        {
            var cachedPaintings = _cacheService.Get<IEnumerable<PaintingDto>>(AvailablePaintingsCacheKey);

            if (cachedPaintings == null)
            {
                _logger.LogInformation("Fetching available paintings from the database");

                var paintings = await _context
                    .Paintings
                    .Include(p => p.PaintingImages.Where(i => i.IsMainImage))
                    .Where(p => p.IsAvailableToSell && !p.IsDeleted && !p.IsOnFocus)
                    .ToListAsync();

                cachedPaintings = _mapper.Map<IEnumerable<PaintingDto>>(paintings);

                // Use default cache settings
                _cacheService.Set(AvailablePaintingsCacheKey, cachedPaintings);
            }

            return Result<IEnumerable<PaintingDto>>.Success(cachedPaintings);
        }


        public async Task<Result<PaintingDto>> GetPaintingById(Guid id)
        {
            var cacheKey = GetPaintingByIdCacheKey(id);

            var cachedPainting = _cacheService.Get<PaintingDto>(cacheKey);

            if (cachedPainting == null)
            {
                _logger.LogInformation($"Fetching painting with ID {id} from the database");

                var painting = await FindPaintingById(id);
                if (painting == null)
                {
                    return Result<PaintingDto>.Fail($"Painting with ID {id} not found.");
                }

                cachedPainting = _mapper.Map<PaintingDto>(painting);

                // Use default cache settings
                _cacheService.Set(cacheKey, cachedPainting);
            }

            return Result<PaintingDto>.Success(cachedPainting);
        }


        public async Task<Result<IEnumerable<PaintingDto>>> GetPortfolioPaintings()
        {
            var cachedPaintings = _cacheService.Get<IEnumerable<PaintingDto>>(PortfolioPaintingsCacheKey);

            if (cachedPaintings == null)
            {
                _logger.LogInformation("Fetching portfolio paintings from the database");

                var paintings = await _context
                    .Paintings
                    .Include(p => p.PaintingImages.OrderBy(i => !i.IsMainImage))
                    .Where(p => !p.IsAvailableToSell && !p.IsDeleted)
                    .Take(6)
                    .ToListAsync();

                cachedPaintings = _mapper.Map<IEnumerable<PaintingDto>>(paintings);

                // Use default cache settings
                _cacheService.Set(PortfolioPaintingsCacheKey, cachedPaintings);
            }

            return Result<IEnumerable<PaintingDto>>.Success(cachedPaintings);
        }


        public async Task<Result<IEnumerable<PaintingDto>>> GetPaintingsOnFocus()
        {
            var cachedPaintings = _cacheService.Get<IEnumerable<PaintingDto>>(PaintingsOnFocusCacheKey);

            if (cachedPaintings == null)
            {
                _logger.LogInformation("Fetching paintings on focus from the database");

                var paintings = await _context
                    .Paintings
                    .Include(p => p.PaintingImages.Where(i => i.IsMainImage))
                    .Where(p => p.IsAvailableToSell && !p.IsDeleted && p.IsOnFocus)
                    .OrderByDescending(p => p.CreatedOn)
                    .ToListAsync();

                cachedPaintings = _mapper.Map<IEnumerable<PaintingDto>>(paintings);

                // Use default cache settings
                _cacheService.Set(PaintingsOnFocusCacheKey, cachedPaintings);
            }

            return Result<IEnumerable<PaintingDto>>.Success(cachedPaintings);
        }

        private static string GetPaintingByIdCacheKey(Guid id)
        {
            return $"Painting_{id}";
        }

        private async Task<Painting> FindPaintingById(Guid id)
        => await _context
            .Paintings.AsNoTracking()
            .Include(p => p.PaintingImages
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.IsMainImage))
            .FirstOrDefaultAsync(p => p.Id == id);

        private async Task<bool> PaintingExists(string name)
        => await _context
            .Paintings
            .AsNoTracking()
            .Where(i => !i.IsDeleted)
            .AnyAsync(i => i.Name == name);

    }
}
