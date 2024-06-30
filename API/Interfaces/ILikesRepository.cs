using API.DTOs;
using API.Entities;
using API.Helper;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<userLike> getUserlike(int sourceUserId,int likedId);
        Task<AppUser> getUserWithLikes(int userId);
        Task<PagedList<LikeDto>> Get_All_User_Likes(LikesParams likesParams);

    }
}
