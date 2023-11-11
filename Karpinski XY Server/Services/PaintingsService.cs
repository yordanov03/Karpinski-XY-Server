using AutoMapper;
using FluentValidation;
using Karpinski_XY.Data;
using Karpinski_XY_Server.Data.Models.Base;
using Karpinski_XY_Server.Data.Models.Painting;
using Karpinski_XY_Server.Dtos.Painting;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY_Server.Services
{
    public class PaintingsService : IPaintingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<PaintingDto> _paintingValidator;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ILogger<PaintingsService> _logger;

        public PaintingsService(ApplicationDbContext context,
            IValidator<PaintingDto> paintingValidator,
            IMapper mapper,
            IFileService fileService,
            ILogger<PaintingsService> logger)
        {
            _context = context;
            _paintingValidator = paintingValidator;
            _mapper = mapper;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<Result<Guid>> Create(PaintingDto model)
        {
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

            _logger.LogInformation("Successfully created a new painting");
            return Result<Guid>.Success(model.Id);
        }

        public async Task<Result<PaintingDto>> GetPaintingToEdit(Guid id)
        {
            _logger.LogInformation($"Fetching painting with id {id} to edit");

            var painting = FindPaintingById(id);
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

            var painting = FindPaintingById(model.Id);
            if (painting == null)
            {
                _logger.LogWarning($"Painting with ID {model.Id} not found.");
                return Result<PaintingDto>.Fail("Painting not found.");
            }

            this._fileService.MarkDeletedImagesAsDeleted(model.PaintingImages, painting.PaintingImages);

            var imagesWithoutPath = model.PaintingImages.Where(i => string.IsNullOrEmpty(i.ImageUrl)).ToList();
            if (imagesWithoutPath.Any())
            {
                await _fileService.UpdateImagePathsAsync(imagesWithoutPath);
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
            painting.PaintingImages.ForEach(image => image.IsDeleted = true);
            _context.Update(painting);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Deleted painting successfully");
            return Result<bool>.Success(true);
        }


        public async Task<Result<IEnumerable<PaintingDto>>> GetAllPaintings()
        {
            _logger.LogInformation("Fetching all paintings");

            var paintings = await _context
                .Paintings
                .Include(p => p.PaintingImages.Where(i=>i.IsMainImage))
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            return Result<IEnumerable<PaintingDto>>.Success(_mapper.Map<IEnumerable<PaintingDto>>(paintings));
        }

        public async Task<Result<IEnumerable<PaintingDto>>> GetAvailablePaintings()
        {
            _logger.LogInformation("Fetching all available paintings");

            var paintings = await _context
                .Paintings
                .Include(p => p.PaintingImages.Where(i => i.IsMainImage))
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
                .Include(p => p.PaintingImages.OrderBy(i => !i.IsMainImage))
                .Where(p => !p.IsAvailableToSell && !p.IsDeleted)
                .Take(6)
                .ToListAsync();

            return Result<IEnumerable<PaintingDto>>.Success(_mapper.Map<IEnumerable<PaintingDto>>(paintings));
        }

        
        public async Task<Result<IEnumerable<PaintingDto>>> GetPaintingsOnFocus()
        {
            _logger.LogInformation($"Getting paintings on focus");

            var paintings = await _context
                .Paintings
                .Include(p => p.PaintingImages.Where(i => i.IsMainImage))
                .Where(p => p.IsAvailableToSell && p.IsDeleted == false && p.IsOnFocus == true)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();

            var mapped = _mapper.Map<List<Painting>, IEnumerable<PaintingDto>>(paintings);
            return Result<IEnumerable<PaintingDto>>.Success(mapped);
        }


        private Painting FindPaintingById(Guid id)
        => _context
            .Paintings
            .Include(p=>p.PaintingImages
                .Where(i=>!i.IsDeleted)
                .OrderBy(i=>!i.IsMainImage))
            .Where(p => p.Id == id)
            .FirstOrDefault();
    }
}
