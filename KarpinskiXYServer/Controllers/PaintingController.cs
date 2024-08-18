using Karpinski_XY_Server.Dtos.Painting;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class PaintingController : ApiController
    {
        private readonly IPaintingsService _paintingsService;

        public PaintingController(IPaintingsService paintingsService)
        {
            _paintingsService = paintingsService;
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
        [DisableRequestSizeLimit]
        [Route("", Name = "create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]PaintingDto model)
        {
            var result = await _paintingsService.Create(model);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [Route("toEdit/{id}", Name = "getPaintingToEdit")]
        [ProducesResponseType(typeof(PaintingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaintingToEdit(Guid id)
        {
            var result = await _paintingsService.GetPaintingToEdit(id);
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpPut]
        [Authorize]
        [AllowAnonymous]
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
        [Authorize]
        [Route("{id}", Name = "delete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
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
        [AllowAnonymous]
        [Route("portfolio", Name = "portfolio")]
        [ProducesResponseType(typeof(IEnumerable<PaintingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPortfolioPaintings()
        {
            var result = await _paintingsService.GetPortfolioPaintings();
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("available", Name = "available")]
        [ProducesResponseType(typeof(IEnumerable<PaintingDto>), StatusCodes.Status200OK)]
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
        [AllowAnonymous]
        [Route("{id}", Name = "loadPainting")]
        [ProducesResponseType(typeof(PaintingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPainting(Guid id)
        {
            var result = await _paintingsService.GetPaintingById(id);
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("", Name = "onFocus")]
        [ProducesResponseType(typeof(IEnumerable<PaintingDto>), StatusCodes.Status200OK)]
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

        [HttpGet]
        [AllowAnonymous]
        [Route("toSell", Name = "toSell")]
        [ProducesResponseType(typeof(IEnumerable<PaintingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllPaintingsToSell()
        {
            var result = await _paintingsService.GetAllPaintingsToSell();
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }
    }
}
