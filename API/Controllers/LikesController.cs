using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository,ILikesRepository likesRepository)
        {
            this._userRepository = userRepository;
            this._likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            // Retrieve the source user ID from the claims
            var sourceUserId = User.GetUserId();

            if (sourceUserId == null)
            {
                return Unauthorized("User ID is missing.");
            }

            // Retrieve the liked user by username
            var likedUser = await _userRepository.GetUserByUserNameAsync(username);
            if (likedUser == null)
            {
                return NotFound("User not found.");
            }

            // Retrieve the source user with their likes
            var sourceUser = await _likesRepository.getUserWithLikes(sourceUserId.Value);
            if (sourceUser == null)
            {
                return NotFound("Source user not found.");
            }

            // Prevent users from liking themselves
            if (sourceUser.UserName == username)
            {
                return Ok();
            }

            // Check if the user already likes the target user
            var userLike = await _likesRepository.getUserlike(sourceUserId.Value, likedUser.Id);
            if (userLike != null)
            {
                return Ok();
            }

            // Create a new like
            userLike = new userLike
            {
                SourceUserId = sourceUserId.Value,
                LikedUserId = likedUser.Id,
            };
            sourceUser.LikedUser.Add(userLike);

            // Save the like to the database
            if (await _userRepository.SaveAllAsync())
            {
                return Ok();
            }

            return BadRequest("Failed to like the user.");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId().Value;
            var users = await _likesRepository.Get_All_User_Likes(likesParams);
            var paginationHeader = new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages );
            
            Response.AddPaginationHeader(paginationHeader);
            
            return Ok(users);
        }
    }
}
