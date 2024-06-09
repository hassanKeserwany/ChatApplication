using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;

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
            var member = await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();

            
                return member;
        }
        public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
        {
            //no need to use include because ProjectTo will get it .
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();
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

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() >0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        
    }
}
