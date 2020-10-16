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
        private readonly ISignInService SignInService;
        private readonly IMapper Mapper;

        public SignInController(ISignInService signInService, IMapper mapper)
        {
            SignInService = signInService;
            Mapper = mapper;
        }

        [HttpPost()]
        public async Task<ActionResult> SignIn(SignInDTO signInDto)
        {
            var userSignIn = Mapper.Map<UserSignIn>(signInDto);
            var (isAuthorize, token) = await SignInService.SignIn(userSignIn);
            if (isAuthorize) return Ok(token);
            return BadRequest();
        }
    }
}