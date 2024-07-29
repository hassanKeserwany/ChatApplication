using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using API.Helper;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context ,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<MemberDto> GetMemberAsync(string username)
        {
            /*            "ProjectTo" : translates object mapping directly into a LINQ query that the database understands.
             *            This means only the necessary fields are selected from the database,
             *            reducing the amount of data transferred and improving performance. 
             *            => only used on the queryable collection (before -> Where)!!!
            */
            //no need to use include because ProjectTo will get it .

            var member = await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();

            
                return member;
        }
        public async Task<PagedList<MemberDto>> GetAllMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            //query = query.Where(u => u.Gender == userParams.Gender);
            var minDOB =DateTime.Today.AddYears(- userParams.MaxAge -1); 
            var maxDOb= DateTime.Today.AddYears(-userParams.MinAge );

            query =query.Where(u=>u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOb);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.CreatedAt),
                "lastActive" => query.OrderByDescending(u => u.LastActive),
                _ => query.OrderBy(u => u.UserName) // Default order
            };
            
            if(userParams.Gender != null)
            {
                query = query.Where(u => u.Gender == userParams.Gender);

            }

            var result = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();
            return await PagedList<MemberDto>.CreateAsync(result, userParams.PageNumber,userParams.PageSize);
        }
        

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users
                        .Include(x=>x.Photos)
                        .SingleOrDefaultAsync(x=>x.Id ==id);
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _context.Users
                         .Include(x=>x.Photos)
                         .SingleOrDefaultAsync(x=>x.UserName == username);

        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                        .Include(x => x.Photos)
                        .ToListAsync();

        }

        

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }


        /*public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }*/


    }
}
