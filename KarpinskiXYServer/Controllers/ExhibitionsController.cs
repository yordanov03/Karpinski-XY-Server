﻿using Karpinski_XY_Server.Dtos.Exhibition;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class ExhibitionsController : ApiController
    {
       private IExhibitionService _exhibitionService;

        public ExhibitionsController(IExhibitionService exhibitionService)
        {
            this._exhibitionService = exhibitionService;
        }

        [HttpPost]
        [Authorize]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        [Route("", Name = "UpdateExhibition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] ExhibitionDto model)
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
        [AllowAnonymous]
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
