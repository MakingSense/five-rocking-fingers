using FRF.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using FRF.Web.Dtos.Users;

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

        /// <summary>
        /// Search an User by email.
        /// </summary>
        /// <param name="email">of the wanted user</param>
        /// <returns>If the email is from a valid user, return a UserPublicProfileDTO</returns>
        // [Authorize] 
        [HttpGet("search")]
        public async Task<IActionResult> SearchUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest();

            /* TODO:Pending AWS Credentials. Login is bypassed!!! [FIVE-6]*/
            //MOCK ::
            var userPublicProfile = new UserPublicProfileDTO();
            if (email.Equals("prueba@makingsense.com"))
            {
                var userPublic = new UserPublicProfileDTO
                {
                    UserId = new Guid("9e9df404-3060-4904-bcb8-020f4344c5f0"),
                    Email = email
                };
                userPublicProfile = userPublic;
            }
            else
            {
                var userPublic = new UserPublicProfileDTO
            {
                UserId = new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba"),
                Email = email
            };
                userPublicProfile = userPublic;
            }
            //END MOCK::
            //Uncomment this after do:
            /*var userId = await _userService.GetUserIdByEmail(email);
            if (userId == null) return NotFound();

            var userPublicProfile = new UserPublicProfileDTO
            {
                FullName = await _userService.GetFullname(email),
                UserId = userId,
                Email = email,
                Avatar = null
            };*/

            return Ok(userPublicProfile);
        }
    }
}