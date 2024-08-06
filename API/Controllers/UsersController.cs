using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using API.Helper;
using API.Extensions;
using API.Data;

namespace API.Controllers
{

    [Authorize] //now all the methods will be protected by authorization
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public IPhotoService _photoService;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService )
        {
            _unitOfWork=unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var user = await _userRepository.GetUserByUserNameAsync(username);

            //userParams.CurrentUserName = user.UserName;
            userParams.CurrentUserName = username;
            //userParams.Gender=User

            var users = await _unitOfWork.UserRepository.GetAllMembersAsync(userParams);

            
            var paginationHeader = new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            Response.AddPaginationHeader(paginationHeader);


            return Ok(users);
        }
        /*[HttpGet]
        public string Get()
        {
            return "hello";
        }*/

        [HttpGet("username/{username}", Name = "GetUserByUsername")]
        public async Task<ActionResult<MemberDto>> GetByUsername(string username)
        {
            var users = await _unitOfWork.UserRepository.GetMemberAsync(username);
            return Ok(users);
        }


        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto UpdatedMember)
        {
            //we should take the username from the token , where we authenticate 
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            
            AppUser user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);

            _mapper.Map(UpdatedMember, user);

            //if we update another time ,we got no error ,
            //because we added a flag in efcore (Update(user)) => say this entity is updated, 
            //Mark the entity as updated
            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete())
            {
                return NoContent();
            }
            return BadRequest("failed to update");
        }


        [HttpGet("id/{id}")]
        public async Task<ActionResult<MemberDto>> GetById(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
            var userToReturn = _mapper.Map<MemberDto>(user);

            return Ok(userToReturn);
        }
        [HttpPost("add-Photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {//we can add only one image , not multiple
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (user.Photos.Count == 0) //if its first photo
                {
                    photo.IsMain = true;
                }
                
            

            user.Photos.Add(photo);
            if (await _unitOfWork.Complete())
            {
                //return _mapper.Map<PhotoDto>(photo); //this is correct , 

                /* but when we create a resource on server , it should return 201 created
                 and inside this respone , it should be (location header)*/
                return CreatedAtRoute("GetUserByUsername", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            }
            return BadRequest("Problem adding Photo ..");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
            {
                return NotFound("Photo not found");
            }

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
            {
                return NotFound("Photo not found");
            }
            if (photo.IsMain)
            {
                return BadRequest("This is a main photo");
            }
            var currentMain= user.Photos.FirstOrDefault(x=>x.IsMain);
            if (currentMain != null) { currentMain.IsMain = false; }
            photo.IsMain = true;

            if(await _unitOfWork.Complete()) return NoContent();
            return BadRequest("failed to set main photo");


        }
    }
}
