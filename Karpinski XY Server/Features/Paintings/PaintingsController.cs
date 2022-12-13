using Karpinski_XY.Features;
using Karpinski_XY.Infrastructure.Services;
using Karpinski_XY_Server.Features.Paintings.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Karpinski_XY_Server.Features.Paintings
{
    public class PaintingsController : ApiController
    {
        private readonly IPaintingsService _paintingsService;

        public PaintingsController(IPaintingsService paintingsService)
        {
            this._paintingsService = paintingsService;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route(nameof(Create))]
        public async Task<ActionResult<Guid>> Create(PaintingDto model)
        {
            var result =  await this._paintingsService.Create(model);

            return Ok();

        }

        [HttpGet]
        [Route("available")]
        public async Task <IEnumerable<PaintingDto>> GetAvailablePaintings()
        {
            return await this._paintingsService.GetAvailablePaitings();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<PaintingDto>> Details(Guid id)
        {
            var painting = await this._paintingsService.GetPaitingById(id);
            return painting;
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult>Update(PaintingDto model)
        {
            var updated = await this._paintingsService.Update(model);

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var paintning = this._paintingsService.Delete(id);
            return Ok(paintning.Result);
        }

        
    }
}
