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
        [Route("", Name = "create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(PaintingDto model)
        {
            var result = await _paintingsService.Create(model);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Route("available", Name = "available")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailablePaintings()
        {
            var result = await _paintingsService.GetAvailablePaintings();
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Route("{id}", Name = "getPainting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _paintingsService.GetPaintingById(id);
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpPut]
        [Route("", Name = "update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(PaintingDto model)
        {
            var result = await _paintingsService.Update(model);
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete]
        [Route("{id}", Name = "delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _paintingsService.Delete(id);
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Route("", Name = "onFocus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaintingsOnFocus()
        {
            var result = await _paintingsService.GetPaintingsOnFocus();
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }
    }
}
