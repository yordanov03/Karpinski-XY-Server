using AutoMapper;
using FluentValidation;
using Karpinski_XY_Server.Data;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Painting;
using Karpinski_XY_Server.Dtos.Painting;
using Karpinski_XY_Server.Services.Contracts;

namespace Karpinski_XY_Server.Services
{
    public class PaintingsService : IPaintingsService
    {
        private const string AllPaintingsToSellCacheKey = "AllPaintingsToSell";
        private const string AvailablePaintingsCacheKey = "AvailablePaintings";
        private const string PortfolioPaintingsCacheKey = "PortfolioPaintings";
        private const string PaintingsOnFocusCacheKey = "PaintingsOnFocus";

        private readonly ICacheService _cacheService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<PaintingDto> _paintingValidator;
        private readonly IMapper _mapper;
        private readonly IFileService<PaintingImageDto> _fileService;
        private readonly ILogger<PaintingsService> _logger;

        public PaintingsService(
            ICacheService cacheService,
            IUnitOfWork unitOfWork,
            IValidator<PaintingDto> paintingValidator,
            IMapper mapper,
            IFileService<PaintingImageDto> fileService,
            ILogger<PaintingsService> logger)
        {
            _cacheService = cacheService;
            _unitOfWork = unitOfWork;
            _paintingValidator = paintingValidator;
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<Result<Guid>> CreateAsync(PaintingDto model)
        {
            if (await _unitOfWork.Paintings.ExistsAsync(model.Name))
            {
                return Result<Guid>.Fail("Painting with that name already exists");
            }

            var validationResult = _paintingValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogError("Validation failed for inquiry. Errors: {ValidationErrors}", string.Join(", ", errorMessages));
                return Result<Guid>.Fail(errorMessages);
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

            await _unitOfWork.Paintings.AddAsync(painting);
            await _unitOfWork.CommitAsync();

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

        public async Task<Result<PaintingDto>> GetPaintingToEditAsync(Guid id)
        {
            _logger.LogInformation($"Fetching painting with id {id} to edit");

            var painting = await _unitOfWork.Paintings.FindByIdAsync(id);
            if (painting == null)
            {
                return Result<PaintingDto>.Fail($"Painting with ID {id} not found.");
            }

            var paintingDto = _mapper.Map<PaintingDto>(painting);

            var imageConversionResult = await _fileService.ConvertImagePathsToBase64Async(paintingDto.PaintingImages);
            if (!imageConversionResult.Succeeded)
            {
                return Result<PaintingDto>.Fail(imageConversionResult.Errors);
            }

            paintingDto.PaintingImages = imageConversionResult.Value;
            return Result<PaintingDto>.Success(paintingDto);
        }

        public async Task<Result<PaintingDto>> UpdateAsync(PaintingDto model)
        {
            var validationResult = _paintingValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogError("Validation failed for inquiry. Errors: {ValidationErrors}", string.Join(", ", errorMessages));
                return Result<PaintingDto>.Fail(errorMessages);
            }

            _logger.LogInformation($"Looking to update painting with id {model.Id}");

            var painting = await _unitOfWork.Paintings.FindByIdAsync(model.Id);
            if (painting == null)
            {
                _logger.LogWarning($"Painting with ID {model.Id} not found.");
                return Result<PaintingDto>.Fail("Painting not found.");
            }

            await _unitOfWork.Paintings.DeleteAsync(model.Id);

            // Create a new record for the updated painting
            await CreateAsync(model);

            await _unitOfWork.CommitAsync();

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


        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            _logger.LogInformation($"Deleting painting with id {id}");

            var painting = await _unitOfWork.Paintings.FindByIdAsync(id);
            if (painting == null)
            {
                _logger.LogWarning($"Painting with ID {id} not found.");
                return Result<bool>.Fail($"Painting with ID {id} not found.");
            }

            await _unitOfWork.Paintings.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            _cacheService.RemoveAll(new[]
            {
        GetPaintingByIdCacheKey(id),
        AllPaintingsToSellCacheKey,
        AvailablePaintingsCacheKey,
        PortfolioPaintingsCacheKey,
        PaintingsOnFocusCacheKey
    });

            _logger.LogInformation("Deleted painting successfully");
            return Result<bool>.Success(true);
        }

        public async Task<Result<PaintingDto>> GetPaintingByIdAsync(Guid id)
        {
            var cacheKey = GetPaintingByIdCacheKey(id);

            // Attempt to retrieve from cache
            var cachedPainting = _cacheService.Get<PaintingDto>(cacheKey);
            if (cachedPainting != null)
            {
                return Result<PaintingDto>.Success(cachedPainting);
            }

            _logger.LogInformation($"Fetching painting with ID {id} from the database");

            // Fetch from repository
            var painting = await _unitOfWork.Paintings.FindByIdAsync(id); // Using the existing repository method
            if (painting == null)
            {
                return Result<PaintingDto>.Fail($"Painting with ID {id} not found.");
            }

            // Map to DTO
            var paintingDto = _mapper.Map<PaintingDto>(painting);

            // Cache the result
            _cacheService.Set(cacheKey, paintingDto);

            return Result<PaintingDto>.Success(paintingDto);
        }


        public async Task<Result<IEnumerable<PaintingDto>>> GetAllPaintingsToSellAsync()
        {
            const string cacheKey = "AllPaintingsToSell";
            return await GetCachedPaintingsAsync(
                cacheKey,
                () => _unitOfWork.Paintings.GetAllToSellAsync(),
                "Fetching all paintings to sell from the database");
        }

        public async Task<Result<IEnumerable<PaintingDto>>> GetAvailablePaintingsAsync()
        {
            const string cacheKey = "AvailablePaintings";
            return await GetCachedPaintingsAsync(
                cacheKey,
                () => _unitOfWork.Paintings.GetAvailableAsync(),
                "Fetching available paintings from the database");
        }

        public async Task<Result<IEnumerable<PaintingDto>>> GetPortfolioPaintingsAsync()
        {
            const string cacheKey = "PortfolioPaintings";
            return await GetCachedPaintingsAsync(
                cacheKey,
                () => _unitOfWork.Paintings.GetPortfolioAsync(),
                "Fetching portfolio paintings from the database");
        }

        public async Task<Result<IEnumerable<PaintingDto>>> GetPaintingsOnFocusAsync()
        {
            const string cacheKey = "PaintingsOnFocus";
            return await GetCachedPaintingsAsync(
                cacheKey,
                () => _unitOfWork.Paintings.GetOnFocusAsync(),
                "Fetching paintings on focus from the database");
        }

        private async Task<Result<IEnumerable<PaintingDto>>> GetCachedPaintingsAsync(
            string cacheKey,
            Func<Task<IEnumerable<Painting>>> fetchFromDatabase,
            string logMessage)
        {
            // Attempt to retrieve from cache
            var cachedPaintings = _cacheService.Get<IEnumerable<PaintingDto>>(cacheKey);

            if (cachedPaintings != null)
            {
                return Result<IEnumerable<PaintingDto>>.Success(cachedPaintings);
            }

            // Fetch from database if not in cache
            _logger.LogInformation(logMessage);
            var paintings = await fetchFromDatabase();

            // Filter main image and map to DTOs
            var filteredPaintings = FilterMainImage(paintings);
            var paintingDtos = _mapper.Map<IEnumerable<PaintingDto>>(filteredPaintings);

            // Cache the result
            _cacheService.Set(cacheKey, paintingDtos);

            return Result<IEnumerable<PaintingDto>>.Success(paintingDtos);
        }

        private static string GetPaintingByIdCacheKey(Guid id)
        {
            return $"Painting_{id}";
        }

        private IEnumerable<Painting> FilterMainImage(IEnumerable<Painting> paintings)
        {
            foreach (var painting in paintings)
            {
                painting.PaintingImages = painting.PaintingImages
                    .Where(i => i.IsMainImage)
                    .ToList();
            }

            return paintings;
        }
    }
}
