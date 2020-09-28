﻿using FRF.Core.Services;
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
        public IUserService UserService { get; set; }

        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        /// <summary>
        /// TEST: Gets the current user fullname searching by email.
        /// </summary>
        /// <returns>A <see cref="String"/> whit the fullname.</returns>
        [HttpGet("getfullnamebyemail")]
        [Authorize]
        public async Task<IActionResult> GetFullNamebyEmail()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                var fullname = await UserService.GetFullnameByEmail(email);
                return Ok(fullname);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// TEST: Gets the current user fullname searching by id.
        /// </summary>
        /// <returns>A <see cref="String"/> whit the fullname.</returns>
        [HttpGet("getfullnamebyid")]
        [Authorize]
        public async Task<IActionResult> GetFullNamebyId()
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var fullname = await UserService.GetFullnameByID(userId);
                return Ok(fullname);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await UserService.Logout();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}