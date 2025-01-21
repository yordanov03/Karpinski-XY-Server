using AutoMapper;
using FluentValidation;
using Karpinski_XY_Server.Data;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Exhibition;
using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Services.Contracts;

namespace Karpinski_XY_Server.Services
{
    public class ExhibitionService : IExhibitionService
    {
        private const string AllExhibitionsCacheKey = "AllExhibitions";
        private static string GetExhibitionByIdCacheKey(Guid id) => $"Exhibition_{id}";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IFileService<ExhibitionImageDto> _fileService;
        private readonly IMapper _mapper;
        private readonly ILogger<ExhibitionService> _logger;
        private readonly IValidator<ExhibitionDto> _exhibitionValidator;

        public ExhibitionService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            IFileService<ExhibitionImageDto> fileService,
            IMapper mapper,
            ILogger<ExhibitionService> logger,
            IValidator<ExhibitionDto> exhibitionValidator)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _fileService = fileService;
            _mapper = mapper;
            _logger = logger;
            _exhibitionValidator = exhibitionValidator;
        }

        public async Task<Result<Guid>> CreateExhibitionAsync(ExhibitionDto model)
        {
            if (await _unitOfWork.Exhibitions.ExistsAsync(model.Title))
            {
                return Result<Guid>.Fail("Exhibition with that name already exists");
            }

            var validationResult = _exhibitionValidator.Validate(model);
            if (!validationResult.IsValid)
            {
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
            await _unitOfWork.Exhibitions.AddAsync(exhibition);
            await _unitOfWork.CommitAsync();

            _cacheService.Remove(AllExhibitionsCacheKey);
            _logger.LogInformation("Successfully created a new exhibition");
            return Result<Guid>.Success(model.Id);
        }
        public async Task<Result<ExhibitionDto>> GetExhibitionToUpdateAsync(Guid id)
        {
            _logger.LogInformation($"Fetching exhibition with id {id} for update");

            // Fetch exhibition using Unit of Work
            var exhibition = await _unitOfWork.Exhibitions.FindByIdAsync(id);
            if (exhibition == null)
            {
                return Result<ExhibitionDto>.Fail($"Exhibition with ID {id} not found.");
            }

            // Map to DTO
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

        public async Task<Result<ExhibitionDto>> UpdateExhibitionAsync(ExhibitionDto model)
        {
            var validationResult = _exhibitionValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                return Result<ExhibitionDto>.Fail(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            _logger.LogInformation($"Updating exhibition with id {model.Id}");

            var exhibition = await _unitOfWork.Exhibitions.FindByIdAsync(model.Id);
            if (exhibition == null)
            {
                return Result<ExhibitionDto>.Fail("Exhibition not found.");
            }

            await _unitOfWork.Exhibitions.Delete(exhibition.Id); // Mark as deleted
            await _unitOfWork.CommitAsync();

            await CreateExhibitionAsync(model); // Recreate with new data

            _cacheService.Remove(GetExhibitionByIdCacheKey(model.Id));
            _cacheService.Remove(AllExhibitionsCacheKey);

            _logger.LogInformation("Exhibition updated successfully");
            return Result<ExhibitionDto>.Success(model);
        }

        public async Task<Result<bool>> DeleteExhibitionAsync(Guid id)
        {
            _logger.LogInformation($"Deleting exhibition with ID {id}");

            var exhibition = await _unitOfWork.Exhibitions.FindByIdAsync(id);
            if (exhibition == null)
            {
                return Result<bool>.Fail($"Exhibition with ID {id} not found.");
            }

            await _unitOfWork.Exhibitions.Delete(exhibition.Id);
            await _unitOfWork.CommitAsync();

            _cacheService.Remove(GetExhibitionByIdCacheKey(id));
            _cacheService.Remove(AllExhibitionsCacheKey);

            _logger.LogInformation("Exhibition deleted successfully");
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<ExhibitionDto>>> GetAllExhibitionsAsync()
        {
            _logger.LogInformation("Fetching all exhibitions");

            var cachedExhibitions = _cacheService.Get<IEnumerable<ExhibitionDto>>(AllExhibitionsCacheKey);
            if (cachedExhibitions != null)
            {
                return Result<IEnumerable<ExhibitionDto>>.Success(cachedExhibitions);
            }

            var exhibitions = await _unitOfWork.Exhibitions.GetAllAsync();
            exhibitions = FilterMainImageForExhibitions(exhibitions).ToList();
            cachedExhibitions = _mapper.Map<IEnumerable<ExhibitionDto>>(exhibitions);

            _cacheService.Set(AllExhibitionsCacheKey, cachedExhibitions);
            _logger.LogInformation("Exhibitions fetched from the database and cached");

            return Result<IEnumerable<ExhibitionDto>>.Success(cachedExhibitions);
        }

        public async Task<Result<ExhibitionDto>> GetExhibitionByIdAsync(Guid id)
        {
            _logger.LogInformation($"Fetching exhibition with ID {id}");

            var exhibition = await _unitOfWork.Exhibitions.FindByIdAsync(id);
            if (exhibition == null)
            {
                return Result<ExhibitionDto>.Fail($"Exhibition with ID {id} not found.");
            }

            return Result<ExhibitionDto>.Success(_mapper.Map<ExhibitionDto>(exhibition));
        }

        private IEnumerable<Exhibition> FilterMainImageForExhibitions(IEnumerable<Exhibition> exhibitions)
        {
            foreach (var exhibition in exhibitions)
            {
                exhibition.ExhibitionImages = exhibition.ExhibitionImages
                    .Where(i => i.IsMainImage)
                    .ToList();
            }

            return exhibitions;
        }
    }
}
