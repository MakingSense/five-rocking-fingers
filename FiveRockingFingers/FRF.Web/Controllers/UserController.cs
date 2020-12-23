﻿using AutoMapper;
using FRF.Core.Services;
using FRF.Web.Dtos.Projects;
using FRF.Web.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FRF.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserPublicProfileAsync()
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            var userPublicProfile = await _userService.GetUserPublicProfile(email);
            if (userPublicProfile == null) return BadRequest();
            
            return Ok(userPublicProfile);
        }

        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return Ok();
        }

        /// <summary>
        /// Search an User by email.
        /// </summary>
        /// <param name="email">of the wanted user</param>
        /// <returns>If the email is from a valid user, return a UserPublicProfileDTO</returns>
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchUserAsync([CustomValidator.EmailPattern] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest();
            
            var userId = await _userService.GetUserPublicProfile(email); 
            if (userId == null) return NotFound();

            var userPublicProfile = _mapper.Map<UserProfileDTO>(userId);
            
            return Ok(userPublicProfile);
        }
    }
}