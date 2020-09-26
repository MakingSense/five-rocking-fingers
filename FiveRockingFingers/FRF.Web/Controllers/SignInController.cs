using System;
using FRF.Core.Models;
using FRF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;
using FRF.Web.Dtos.Users;

namespace FRF.Web.Controllers
{
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

        [HttpPost]
        public async Task<ActionResult<string>> SignIn(SignInDTO signInDto)
        {
            var userSignIn = Mapper.Map<UserSignIn>(signInDto);
            try
            {
                var token =await SignInService.SignIn(userSignIn);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }
    }
}