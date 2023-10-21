using Karpinski_XY.Features;
using Karpinski_XY_Server.Features.Paintings.Models;
using Karpinski_XY_Server.Features.Paintings.Services;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Features.Paintings
{
    public class PaintingsController : ApiController
    {
        private readonly IPaintingsService _paintingsService;

        public PaintingsController(IPaintingsService paintingsService)
        {
            this._paintingsService = paintingsService;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("", Name ="create")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> Create(PaintingDto model)
        {
            await this._paintingsService.Create(model);
            return Ok();
        }

        [HttpGet]
        [Route("available", Name = "available")]
        [ProducesResponseType(typeof(List<PaintingDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<PaintingDto>> GetAvailablePaintings()
        {
            return await this._paintingsService.GetAvailablePaintings();
        }

        [HttpGet]
        [Route("{id}", Name = "getPainting")]
        [ProducesResponseType(typeof(PaintingDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaintingDto>> Details(Guid id)
        {
            return await this._paintingsService.GetPaintingById(id);
        }

        [HttpPut]
        [Route("", Name = "update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Update(PaintingDto model)
        {
            var updated = await this._paintingsService.Update(model);
            return Ok(updated);
        }

        [HttpDelete]
        [Route("{id}", Name = "delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var painting = await this._paintingsService.Delete(id);
            return Ok(painting);
        }

        [HttpGet]
        [Route("", Name = "onFocus")]
        [ProducesResponseType(typeof(List<PaintingDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<PaintingDto>> GetPaintingsOnFocus()
        {
            return await this._paintingsService.GetPaintingsOnFocus();
        }
    }
}
