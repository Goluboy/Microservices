using AuthMicroservice.API.Dtos;
using AuthMicroservice.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroservice.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController(RoleManager<RoleDal> roleManager) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(roleManager.Roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            if (await roleManager.RoleExistsAsync(request.Name))
            {
                return BadRequest(new { message = "Role already exists" });
            }
            var role = new RoleDal
            {
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok(role);
            }
            return BadRequest(result.Errors);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteRole(Guid Id)
        {
            var role = await roleManager.FindByIdAsync(Id.ToString());
            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }
            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
    }
}
