﻿using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AwsArtifactsProviderController : ControllerBase
    {
        private readonly IArtifactsProviderService _artifactsProviderService;
        private readonly IMapper _mapper;

        public AwsArtifactsProviderController(IArtifactsProviderService artifactsProviderService, IMapper mapper)
        {
            _artifactsProviderService = artifactsProviderService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieve all the artifact names from AWS provider.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNamesAsync()
        {
            var productsNames = await _artifactsProviderService.GetNamesAsync();
            if (productsNames==null)
            {
                return StatusCode(503);
            }
            return Ok(productsNames);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttributesAsync(string serviceCode)
        {
            var attributes = await _artifactsProviderService.GetAttributesAsync(serviceCode);

            return Ok(_mapper.Map<List<ProviderArtifactSettingDTO>>(attributes));
        }

        [HttpPost]
        public async Task<IActionResult> GetProductsAsync(List<KeyValuePair<string, string>> settings, string serviceCode)
        {
            var products = await _artifactsProviderService.GetProductsAsync(settings, serviceCode);

            return Ok(_mapper.Map<List<PricingTermDTO>>(products));
        }
    }
}
