﻿using System;
using FRF.Core.Models;
using FRF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using FRF.Web.Dtos.Users;
using Microsoft.AspNetCore.Authorization;

namespace FRF.Web.Controllers
{ 
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        public ISignInService SignInService { get; set; }
        public IMapper Mapper { get; set; }

        public SignInController(ISignInService signInService, IMapper mapper)
        {
            SignInService = signInService;
            Mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> SignIn(SignInDTO signInDto)
        {
            var userSignIn = Mapper.Map<UserSignIn>(signInDto);
            try
            {
                await SignInService.SignIn(userSignIn);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}