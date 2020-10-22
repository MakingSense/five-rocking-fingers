using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly ISignInService _signInService;
        private readonly IMapper _mapper;

        public SignInController(ISignInService signInService, IMapper mapper)
        {
            _signInService = signInService;
            _mapper = mapper;
        }

        [HttpPost()]
        public async Task<ActionResult> SignIn(SignInDTO signInDto)
        {
            var userSignIn = _mapper.Map<UserSignIn>(signInDto);
            var (isAuthorize, token) = await _signInService.SignIn(userSignIn);
            if (isAuthorize) return Ok(token);
            return BadRequest();
        }
    }
}