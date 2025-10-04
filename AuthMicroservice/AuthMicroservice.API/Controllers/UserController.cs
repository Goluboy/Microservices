using AuthMicroservice.API.Dtos;
using AuthMicroservice.DAL.Models;
using AuthMicroservice.Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuthMicroservice.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(UserManager<UserDal> userManager, SignInManager<UserDal> signInManager, IJwtService jwtService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await userManager.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();


            var usersWithRoles = new List<UsersWithRoles>();
            var response = new GetUsersResponse(usersWithRoles);

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                usersWithRoles.Add(new(user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    roles));
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
        {
            var user = new UserDal
            {
                UserName = request.Email,
                Email = request.Email
            };
           var result = await userManager.CreateAsync(user, request.Password);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return BadRequest(new { message = "Account is locked out" });
                }

                if (result.IsNotAllowed)
                {
                    return BadRequest(new { message = "Login not allowed. Please confirm your email." });
                }

                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = await jwtService.GenerateAccessTokenAsync(user);

            return Ok(token);
        }

        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] string accessToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest("Invalid input");
            }

            var (result, claimsPrincipal) = jwtService.ValidateTokenAsync(accessToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDal user)
        {
            var existingUser = await userManager.FindByIdAsync(user.Id.ToString());
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;

            var result = await userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(existingUser);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            await userManager.DeleteAsync(await userManager.FindByIdAsync(id.ToString()));
            return NoContent();
        }

        [HttpPut("assignRole")]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            await userManager.AddToRoleAsync(user, request.Role);
            return Ok(user);
        }
    }
}