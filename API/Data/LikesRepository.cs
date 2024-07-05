using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<userLike> getUserlike(int sourceUserId, int targetUserId)
        {
             return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> Get_All_User_Likes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked") { 
                likes =likes.Where(like=>like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);//get users who i  liked
            }

            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }
            var likedUsers= users.Select(user => new LikeDto()
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age =user.DateOfBirth.CalculateAge(),
                PhotoUrl=user.Photos.FirstOrDefault(p=>p.IsMain).Url,
                City=user.City,
                Id=user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber,likesParams.PageSize);

        }

        public async Task<AppUser> getUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUser)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        
    }
}