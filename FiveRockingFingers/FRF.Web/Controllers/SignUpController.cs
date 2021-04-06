using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
   
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISignUpService _signUpService;

        public SignUpController(IMapper mapper, ISignUpService signUpService)
        {
            _mapper = mapper;
            _signUpService = signUpService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> SignUp(SignUpDTO signUpDto)
        {
            var userSignUp = _mapper.Map<User>(signUpDto);
            var response = await _signUpService.SignUpAsync(userSignUp);
            if (!response.Success && response.Error.Code == ErrorCodes.InvalidCredentials) return BadRequest($"Error {response.Error.Code}: {response.Error.Message}");
            if (!response.Success && response.Error.Code == ErrorCodes.AuthenticationServerCurrentlyUnavailable) return StatusCode(500,($"Error {response.Error.Code}: {response.Error.Message}"));
            return Ok(response.Value);
        }
    }
}