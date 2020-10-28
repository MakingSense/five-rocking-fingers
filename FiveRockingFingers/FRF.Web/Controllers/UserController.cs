using FRF.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /* TODO:Pending AWS Credentials. Login is bypassed!!! [FIVE-6]*/
        // [Authorize] 
        [HttpGet("getfullname")]
        public async Task<IActionResult> GetFullName()
        {
            /*Clear this after do*/
            return Ok("FRF Developers");
            /**************************/
            /*Uncomment this after do.
            /*
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                var fullname = await _userService.GetFullname(email).ConfigureAwait(false);
                if (fullname == null)
                {
                    return BadRequest();
                }
                else if (fullname == "")
                {
                    return NotFound();
                }
                return Ok(fullname);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }*/
        }

        /*Uncomment this after do.
        /*
        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            
            try
            {
                await _userService.Logout();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }*/
    }
}