using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context , ITokenService tokenService)
        {
            _context = context;
           _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>>Register(RegisterDto registerDto)
        {
            if(await UserExist(registerDto.Username) )
            {
                return BadRequest("username is taken");
            }
            using var hmac =new HMACSHA512();
            var user = new AppUser()
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash =hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key,
                DateOfBirth = registerDto.DateOfBirth,
                KnownAs = registerDto.KnownAs,
                CreatedAt = registerDto.CreatedAt,
                LastActive=registerDto.LastActive,
                Gender = registerDto.Gender,
                Introduction=registerDto.Introduction,
                Lookingfor = registerDto.Lookingfor,
                Interests = registerDto.Interests,
                City = registerDto.City,
                Country = registerDto.Country,
                
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                UserName = user.UserName,
                Token =_tokenService.CreateToken(user)
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user=await _context.Users.SingleOrDefaultAsync(x=>x.UserName==loginDto.Username);

            if(user==null)
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

                Token = _tokenService.CreateToken(user)
            };
        }



        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x=>x.UserName.ToLower() == username.ToLower());
        }
    }
}
