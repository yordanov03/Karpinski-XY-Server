using AutoMapper;
using Karpinski_XY.Data;
using Karpinski_XY.Infrastructure.Services;
using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Karpinski_XY_Server.Features.Paintings
{
    public class PaintingsService : IPaintingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PaintingsService(ApplicationDbContext context, 
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<Guid> Create(PaintingDto model)
        {
            var painting = this._mapper.Map<Painting>(model);
            this._context.Add(painting);
            await this._context.SaveChangesAsync();
            return painting.Id;
        }

        public async Task<IEnumerable<PaintingDto>> GetAllPaitings()
        {
            var paintings = await this._context
                .Paintings
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            return this._mapper.Map<IEnumerable<PaintingDto>>(paintings);
        }

        public async Task<IEnumerable<PaintingDto>> GetAvailablePaitings()
        {
            var paintings = await this._context
                .Paintings
                .Where(p => p.IsAvailableToSell && p.IsDeleted == false)
                .ToListAsync();

           var mapped = this._mapper.Map<List<Painting>,IEnumerable<PaintingDto>>(paintings);
            return mapped;
        }

        public async Task<PaintingDto> GetPaitingById(Guid id)
        {
            var painting = FindPaintingById(id);
           return this._mapper.Map<PaintingDto>(painting);
        }


        public async Task<IEnumerable<PaintingDto>> GetPortfolioPaitings()
        {
            var paintings = await this._context
                .Paintings
                .Where(p => !p.IsAvailableToSell)
                .ToListAsync();

            return this._mapper.Map<IEnumerable<PaintingDto>>(paintings);
        }

        public async Task<Guid> Update(PaintingDto model)
        {
            var painting = FindPaintingById(model.Id);
            var updatedPainting = _mapper.Map<Painting>(model);
     
            this._context.Update(updatedPainting);
            await this._context.SaveChangesAsync();
            return painting.Id;
        }

        public async Task<Result> Delete(Guid id)
        {
            var painting = this.FindPaintingById(id);
            painting.IsDeleted = true;
            this._context.Update(painting);
            await this._context.SaveChangesAsync();
            return true;
        }

        private Painting FindPaintingById(Guid id)
        => this._context
            .Paintings
            .Where(p => p.Id == id)
            .FirstOrDefault();
    }
}
