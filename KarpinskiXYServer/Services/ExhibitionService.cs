using AutoMapper;
using FluentValidation;
using Karpinski_XY.Data;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Exhibition;
using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY_Server.Services
{
    public class ExhibitionService : IExhibitionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<ExhibitionDto> _exhibitionValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ExhibitionService> _logger;
        private readonly IFileService<ExhibitionImageDto> _fileService;
        private readonly ICacheService _cacheService;

        private const string AllExhibitionsCacheKey = "AllExhibitions";
        private static string GetExhibitionByIdCacheKey(Guid id) => $"Exhibition_{id}";

        public ExhibitionService(
            ApplicationDbContext context,
            IValidator<ExhibitionDto> exhibitionValidator,
            IMapper mapper,
            ILogger<ExhibitionService> logger,
            IFileService<ExhibitionImageDto> fileService,
            ICacheService cacheService)
        {
            _context = context;
            _exhibitionValidator = exhibitionValidator;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
            _cacheService = cacheService;
        }

        public async Task<Result<Guid>> CreateExhibition(ExhibitionDto model)
        {
            if (await ExhibitionExists(model.Title))
            {
                return Result<Guid>.Fail("Exhibition with that name already exists");
            }

            var validationResult = _exhibitionValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogError("Validation failed for exhibition creation. Errors: {ValidationErrors}", string.Join(", ", errorMessages));
                return Result<Guid>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            _logger.LogInformation("Creating a new exhibition");

            model.Id = Guid.NewGuid();

            var updateResult = await _fileService.UpdateImagePathsAsync(model.ExhibitionImages);

            if (!updateResult.Succeeded)
            {
                return Result<Guid>.Fail("Failed to update image paths.");
            }

            model.ExhibitionImages = updateResult.Value;

            var exhibition = _mapper.Map<Exhibition>(model);
            _context.Add(exhibition);
            await _context.SaveChangesAsync();

            _cacheService.Remove(AllExhibitionsCacheKey);
            _logger.LogInformation("Successfully created a new exhibition");
            return Result<Guid>.Success(model.Id);
        }

        public async Task<Result<bool>> DeleteExhibition(Guid id)
        {
            _logger.LogInformation($"Deleting exhibition with ID {id}");

            var exhibition = await FindExhibitionById(id);
            if (exhibition == null)
            {
                _logger.LogWarning($"Exhibition with ID {id} not found.");
                return Result<bool>.Fail($"Exhibition with ID {id} not found.");
            }

            exhibition.IsDeleted = true;
            exhibition.ExhibitionImages.ForEach(image => image.IsDeleted = true);
            _context.Update(exhibition);
            await _context.SaveChangesAsync();

            _cacheService.Remove(GetExhibitionByIdCacheKey(id));
            _cacheService.Remove(AllExhibitionsCacheKey);
            _logger.LogInformation("Exhibition deleted successfully");
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<ExhibitionDto>>> GetAllExhibitions()
        {
            _logger.LogInformation("Fetching all exhibitions");

            var cachedExhibitions = _cacheService.Get<IEnumerable<ExhibitionDto>>(AllExhibitionsCacheKey);

            if (cachedExhibitions == null)
            {
                var exhibitions = await _context
                    .Exhibitions
                        .Include(e => e.ExhibitionImages
                        .Where(e => e.IsMainImage))
                        .Where(e => !e.IsDeleted)
                    .ToListAsync();

                cachedExhibitions = _mapper.Map<IEnumerable<ExhibitionDto>>(exhibitions);

                _cacheService.Set(AllExhibitionsCacheKey, cachedExhibitions);
                _logger.LogInformation("Exhibitions fetched from the database and cached.");
            }
            else
            {
                _logger.LogInformation("Exhibitions fetched from the cache.");
            }

            return Result<IEnumerable<ExhibitionDto>>.Success(cachedExhibitions);
        }


        public async Task<Result<ExhibitionDto>> GetExhibitionById(Guid id)
        {
            _logger.LogInformation($"Fetching exhibition with ID {id}");

            var exhibition = FindExhibitionById(id);
            if (exhibition == null)
            {
                return Result<ExhibitionDto>.Fail($"Exhibition with ID {id} not found.");
            }

            return Result<ExhibitionDto>.Success(_mapper.Map<ExhibitionDto>(exhibition));
        }

        public async Task<Result<ExhibitionDto>> GetExhibitionToUpdate(Guid id)
        {
            _logger.LogInformation($"Fetching exhibition with id {id} for update");

            var exhibition = FindExhibitionById(id);
            if (exhibition == null)
            {
                return Result<ExhibitionDto>.Fail($"Exhibition with ID {id} not found.");
            }

            var exhibitionDto = _mapper.Map<ExhibitionDto>(exhibition);


            // Convert image paths to Base64 strings
            var imageConversionResult = await _fileService.ConvertImagePathsToBase64Async(exhibitionDto.ExhibitionImages);
            if (!imageConversionResult.Succeeded)
            {
                return Result<ExhibitionDto>.Fail(imageConversionResult.Errors);
            }

            exhibitionDto.ExhibitionImages = imageConversionResult.Value;

            return Result<ExhibitionDto>.Success(exhibitionDto);
        }

        public async Task<Result<ExhibitionDto>> UpdateExhibition(ExhibitionDto model)
        {
            var validationResult = _exhibitionValidator.Validate(model);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogError("Validation failed for exhibition update. Errors: {ValidationErrors}", string.Join(", ", errorMessages));
                return Result<ExhibitionDto>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            _logger.LogInformation($"Updating exhibition with id {model.Id}");

            var exhibition = await FindExhibitionById(model.Id);
            if (exhibition == null)
            {
                _logger.LogWarning($"Exhibition with ID {model.Id} not found.");
                return Result<ExhibitionDto>.Fail("Exhibition not found.");
            }

            exhibition.ExhibitionImages.ForEach(i => i.IsDeleted = true);
            _context.UpdateRange(exhibition.ExhibitionImages);
            exhibition.IsDeleted = true;
            _context.Update(exhibition);
            await _context.SaveChangesAsync();

            await CreateExhibition(model);

            _cacheService.Remove(GetExhibitionByIdCacheKey(model.Id));
            _cacheService.Remove(AllExhibitionsCacheKey);

            _logger.LogInformation("Exhibition updated successfully");
            return Result<ExhibitionDto>.Success(model);
        }

        private async Task <Exhibition> FindExhibitionById(Guid id)
        => await _context
             .Exhibitions
             .Include(p => p.ExhibitionImages
             .Where(i => !i.IsDeleted)
             .OrderBy(i => !i.IsMainImage))
             .Where(p => p.Id == id)
             .FirstOrDefaultAsync();

        private async Task<bool> ExhibitionExists(string title)
        => await _context
            .Exhibitions
            .Where(i => !i.IsDeleted)
            .AnyAsync(i => i.Title == title);
    }
}
