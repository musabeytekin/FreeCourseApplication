﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.IdentityServer.DTOs;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.DTOs;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.IdentityServer.Controllers
{
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]    
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto userSignUpDto)
        {
            var user = new ApplicationUser
            {
                UserName = userSignUpDto.UserName,
                Email = userSignUpDto.Email,
                City = userSignUpDto.City
            };

            var result = await _userManager.CreateAsync(user, userSignUpDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
            }

            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
           var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

           if (userIdClaim is null)
           {
               return BadRequest();
           }
           
           var user = await _userManager.FindByIdAsync(userIdClaim.Value);

           if (user is null)
           {
               return BadRequest();
           }

           return Ok(new
           {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City
           });
        }
    }
}

