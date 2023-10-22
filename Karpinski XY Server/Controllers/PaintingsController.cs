﻿using Karpinski_XY_Server.Dtos;
using Karpinski_XY_Server.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karpinski_XY_Server.Controllers
{
    public class PaintingsController : ApiController
    {
        private readonly IPaintingsService _paintingsService;

        public PaintingsController(IPaintingsService paintingsService)
        {
            _paintingsService = paintingsService;
        }

        [HttpPost]
        //[Authorize]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [Authorize]
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
        [AllowAnonymous]
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