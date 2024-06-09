using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [Authorize] //now all the methods will be protected by authorization
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repo ,IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _repo.GetAllMembersAsync();
            //var usersToReturn =_mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(users);
        }


        [HttpGet("username/{username}")]
        public async Task<ActionResult<MemberDto>> GetByUsername(string username)
        {
            var user = await _repo.GetMemberAsync(username);

            return Ok(user);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto UpdatedMember )
        {
            //we should take the username from the token , where we authenticate 
            var username=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            AppUser user = await _repo.GetUserByUserNameAsync(username);
            
            _mapper.Map(UpdatedMember, user);

            //if we update another time ,we got no error ,
            //because we added a flag in efcore (Update(user)) => say this entity is updated, 
            //Mark the entity as updated
            _repo.Update(user);

            if (await _repo.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("failed to update");
        }


        [HttpGet("id/{id}")]
        public async Task<ActionResult<MemberDto>> GetById(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            var userToReturn = _mapper.Map<MemberDto>(user);

            return Ok(userToReturn);
        }
        
    }
}
