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
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class SignUpController : ControllerBase
    {
        private readonly ISignUpService _signUpService;
        private readonly IMapper _mapper;

        public SignUpController(ISignUpService signUpService, IMapper mapper)
        {
            _signUpService = signUpService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<string>> SignUp(SignUpDTO signUpDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userSignUp = _mapper.Map<User>(signUpDto);
            var (isAuthorize, token) = await _signUpService.SignUp(userSignUp);
            if (!isAuthorize) return BadRequest();
            return Ok(token);
        }
    }
}