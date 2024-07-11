using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }


        [Authorize(Policy ="RequiredAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users
                .Include(x=>x.UserRoles)
                .ThenInclude(r=>r.Role).OrderBy(x=>x.UserName)
                .Select(u =>new
                {
                    u.Id,
                    username = u.UserName,
                    Roles=u.UserRoles.Select(x=>x.Role.Name).ToList(),
                })
                .ToListAsync();
            return Ok(users);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost ("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles =roles.Split(',').ToArray();

            var user =await userManager.FindByNameAsync(username);
            if (user == null) { return NotFound("no user found"); }

            var userRoles = await userManager.GetRolesAsync(user);

            var result =await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) { return BadRequest("failed to add roles"); }

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if(!result.Succeeded) { return BadRequest("failed to remove from roles"); }

            return Ok(await userManager.GetRolesAsync(user) ) ;
        }



        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("Photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            return Ok();
        }
    }
}
