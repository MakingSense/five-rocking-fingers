using System.Security.Claims;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FRF.Web.Dtos.Projects;

namespace FRF.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        private readonly ISignInService _signInService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public SignInController(ISignInService signInService, IMapper mapper, IUserService userService)
        {
            _signInService = signInService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost()]
        public async Task<ActionResult> SignIn(SignInDTO signInDto)
        {
            var userSignIn = _mapper.Map<UserSignIn>(signInDto);
            var response = await _signInService.SignInAsync(userSignIn);
            if (!response.Success && response.Error.Code == ErrorCodes.InvalidCredentials) return Unauthorized();
            if (!response.Success && response.Error.Code == ErrorCodes.AuthenticationServerCurrentlyUnavailable) return StatusCode(500);

            return Ok(response.Value);
        }
    }
}