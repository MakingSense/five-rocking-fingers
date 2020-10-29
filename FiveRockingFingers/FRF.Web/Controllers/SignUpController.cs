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
        /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
        /*Uncomment this after do.*/
        /*
        private readonly IMapper _mapper;
        private readonly ISignUpService _signUpService;

        public SignUpController(IMapper mapper)
        {
            _signUpService = signUpService;
            _mapper = mapper;
        }

       
        [HttpPost]
        public async Task<ActionResult<string>> SignUp(SignUpDTO signUpDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userSignUp = _mapper.Map<User>(signUpDto);
            var (isAuthorize, token) = await _signUpService.SignUpAsync(userSignUp);
            if (!isAuthorize) return BadRequest();
            return Ok(token);
        }*/
    }
}