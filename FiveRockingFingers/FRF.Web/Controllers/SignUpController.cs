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
        private readonly ISignUpService SignUpService;
        private readonly IMapper Mapper;

        public SignUpController(ISignUpService signUpService, IMapper mapper)
        {
            SignUpService = signUpService;
            Mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<string>> SignUp(SignUpDTO signUpDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userSignUp = Mapper.Map<User>(signUpDto);
            var (isAuthorize, token) = await SignUpService.SignUp(userSignUp);
            if (!isAuthorize) return BadRequest();
            return Ok(token);
        }
    }
}