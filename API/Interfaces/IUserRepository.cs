using API.DTOs;
using API.Entities;
using API.Helper;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameAsync(string username);
        Task<PagedList<MemberDto>> GetAllMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username);
        //Task<bool> SaveAllAsync(); => unit of work will do this


    }
}
