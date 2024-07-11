using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,ITokenService tokenService, IMapper mapper)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
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

            user.UserName = registerDto.Username.ToLower();


            //using var hmac = new HMACSHA512();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            //user.PasswordSalt = hmac.Key;

            /*
             remove PasswordHash PasswordSalt
             because we will use Identity from Microsoft.AspNetCore.Identity 
            */



            user.Interests = "";
            user.Introduction = "";
            user.Lookingfor = "";



            /*_context.Users.Add(user);
            await _context.SaveChangesAsync();*/

            var result = await _userManager.CreateAsync(user,registerDto.Password);

            if(!result.Succeeded) { return BadRequest(result.Errors); }

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) { return BadRequest(result.Errors); }


            return new UserDto
            {
                UserName = user.UserName,
                Token =await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                    .Include(u => u.Photos) // Eagerly load the photos collection
                    .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if (user==null)
            {
                return Unauthorized("no user found");
            }
            
            /*var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0; i<computedHash.Length;i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }*/
            // remove PasswordHash PasswordSalt
            // because we will use Identity from Microsoft.AspNetCore.Identity;

            var result=await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
            if (!result.Succeeded) { return Unauthorized(); }

            return new UserDto
            {
                UserName = user.UserName,
                Token =await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender

            };
        }



        private async Task<bool> UserExist(string username)
        {
            return await _userManager.Users.AnyAsync(x=>x.UserName.ToLower() == username.ToLower());
        }
    }
}
