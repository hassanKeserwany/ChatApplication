using API.Data;
using API.DTOs;
using API.Entities;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context , ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.Username))
            {
                return BadRequest("username is taken");
            }
            var user = _mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            user.Interests = "";
            user.Introduction = "";
            user.Lookingfor = "";
            



            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                UserName = user.UserName,
                Token =_tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                    .Include(u => u.Photos) // Eagerly load the photos collection
                    .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user==null)
            {
                return Unauthorized("no user found");
            }
            using var hmac =new HMACSHA512(user.PasswordSalt);
            var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0; i<computedHash.Length;i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }
            return new UserDto
            {
                UserName = user.UserName,

                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender

            };
        }



        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x=>x.UserName.ToLower() == username.ToLower());
        }
    }
}
