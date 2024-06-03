using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System;
using API.Interfaces;
using API.DTOs;
using AutoMapper;

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
        public async Task<ActionResult<MemberDto>> GetById(string username)
        {
            var user = await _repo.GetMemberAsync(username);
            //var userToReturn = _mapper.Map<MemberDto>(user);

            return Ok(user);
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
