using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class ExhibitionController : ApiController
    {
       private IExhibitionService _exhibitionService;

        public ExhibitionController(IExhibitionService exhibitionService)
        {
            this._exhibitionService = exhibitionService;
        }

        [HttpPost]
        [Authorize]
        [DisableRequestSizeLimit]
        [Route("", Name = "CreateExhibition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ExhibitionDto model)
        {
            var result = await _exhibitionService.CreateExhibition(model);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Authorize]
        [Route("{id}", Name = "GetExhibition")]
        [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetExhibition(Guid id)
        {
            var result = await _exhibitionService.GetExhibitionById(id);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Authorize]
        [Route("toEdit/{id}", Name = "GetExhibitionToEdit")]
        [ProducesResponseType(typeof(ExhibitionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetExhibitionToEdit(Guid id)
        {
            var result = await _exhibitionService.GetExhibitionToUpdate(id);
            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Authorize]
        [Route("", Name = "GetAllExhibitions")]
        [ProducesResponseType(typeof(IEnumerable<ExhibitionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllExhibitions()
        {
            var result = await _exhibitionService.GetAllExhibitions();

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpPut]
        [Authorize]
        [Route("{id}", Name = "UpdateExhibition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ExhibitionDto model)
        {
            var result = await _exhibitionService.UpdateExhibition(model);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}", Name = "DeleteExhibition")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _exhibitionService.DeleteExhibition(id);

            if (result.Succeeded)
            {
                return Ok(result.Value);
            }

            return BadRequest(result.Errors);
        }

    }
}
