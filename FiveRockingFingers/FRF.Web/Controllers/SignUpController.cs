using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos.Users;
using Microsoft.AspNetCore.Authentication;
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
        public SignUpController(ISignUpService signUpService, IMapper mapper)
        {
            SignUpService = signUpService;
            Mapper = mapper;
        }

        public ISignUpService SignUpService { get; set; }
        public IMapper Mapper { get; set; }

        [HttpPost]
        public async Task<ActionResult<string>> SignUp(SignUpDTO signUpDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userSignUp = Mapper.Map<User>(signUpDto);
            try
            {
                await SignUpService.SignUp(userSignUp);
                var token = await HttpContext.GetTokenAsync("id_token");
                return Ok(token);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}