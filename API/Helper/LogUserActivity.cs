using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Helper
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            // Check if user is authenticated
            if (! resultContext.HttpContext.User.Identity.IsAuthenticated)
                return;

            // Retrieve the user ID from the claims
            var userIdClaim = resultContext.HttpContext.User.GetUserId();
            var userId =userIdClaim.Value;

            // Get repository service
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

            // Get user from repository by user ID
            var user = await repo.GetUserByIdAsync(userId);

            if (user != null)
            {
                // Update user's last active timestamp
                user.LastActive = DateTime.Now;
                await repo.SaveAllAsync();
            }
            else
            {
                Console.WriteLine($"User with ID {userId} not found.");
            }
        }
    }
}
