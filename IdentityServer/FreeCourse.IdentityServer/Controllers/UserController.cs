using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.IdentityServer.DTOs;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FreeCourse.IdentityServer.Controllers
{
    [Route("api/[controller]")]
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
    }
}

