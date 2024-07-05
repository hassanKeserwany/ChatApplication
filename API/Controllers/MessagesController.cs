using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMessageRepository messageRepository,IUserRepository userRepository, IMapper mapper)
        {
            this._messageRepository = messageRepository;
            this._userRepository = userRepository;
            this._mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMesssageDto createMesssageDto)
        {
            var username = User.GetUsername();

            if (username == createMesssageDto.RecipientUsername.ToLower())
            {
                //return BadRequest("You cannot send messages to yourself.");
            }
            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMesssageDto.RecipientUsername);
            
            if (recipient == null) return NotFound("Recipient not found");

            var message = new Message
            {
                Sender = sender,
                SenderUserName = sender.UserName,
                Recipient = recipient,
                RecipientUserName = recipient.UserName,
                content = createMesssageDto.Content,                

            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                return Ok(_mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUserAsync(messageParams);
            var paginationHeader = new PaginationHeader(messages.CurrentPage, messages.PageSize,
                                                        messages.TotalCount, messages.TotalPages);

            Response.AddPaginationHeader(paginationHeader);

            return messages;
        }



        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            if (currentUsername == null)
            {
                return Unauthorized();
            }

            var messages = await _messageRepository.GetMessageThreadAsync(currentUsername, username);

            if (messages == null)
            {
                return NotFound();
            }

            return Ok(messages);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            // Get the username of the currently logged-in user
            var username = User.GetUsername();

            // Retrieve the message from the repository
            var message = await _messageRepository.GetMessageAsync(id);

            // Check if the message exists
            if (message == null)  return NotFound("Message not found");


            // Check if the user is authorized to delete the message
            //only the sender or the recipient should have the authority to delete the message.


            /*if (message.SenderUserName != username && message.RecipientUserName != username)
                return Unauthorized("You are not authorized to delete this message");*/
            
            //or
            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized("You are not authorized to delete this message");


            if (message.SenderUserName ==username) message.SenderDeleted = true;

            if (message.RecipientUserName == username) message.RecipientDeleted = true;

            // Delete the message
            if (message.SenderDeleted && message.RecipientDeleted) { 
                _messageRepository.DeleteMessage(message);
            }
            
            

            // Save changes to the database
            if (await _messageRepository.SaveAllAsync())
                return Ok();

            // Return a bad request if the message couldn't be deleted
            return BadRequest("Failed to delete the message");
        }



    }
}
