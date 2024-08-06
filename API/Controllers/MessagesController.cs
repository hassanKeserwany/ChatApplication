using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    public class MessagesController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IUnitOfWork unitOfWork,IMapper mapper ,ILogger<MessagesController> logger)
             
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMesssageDto createMesssageDto)
        {
            var username = User.GetUsername();

            if (username == createMesssageDto.RecipientUsername.ToLower())
            {
                //return BadRequest("You cannot send messages to yourself.");
            }
            var sender = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            var recipient = await _unitOfWork.UserRepository.GetUserByUserNameAsync(createMesssageDto.RecipientUsername);
            
            if (recipient == null) return NotFound("Recipient not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                content = createMesssageDto.Content,                

            };

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
            

            return BadRequest("Failed to send message");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
            var paginationHeader = new PaginationHeader(messages.CurrentPage, messages.PageSize,
                                                        messages.TotalCount, messages.TotalPages);

            Response.AddPaginationHeader(paginationHeader);

            return messages;
        }



        /*[HttpGet("thread/{username}")]
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
        }*/


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            // Get the username of the currently logged-in user
            var username = User.GetUsername();

            // Retrieve the message from the repository
            var message = await _unitOfWork.MessageRepository.GetMessage(id);

            // Check if the message exists
            if (message == null)  return NotFound("Message not found");


            // Check if the user is authorized to delete the message
            //only the sender or the recipient should have the authority to delete the message.


            /*if (message.SenderUserName != username && message.RecipientUserName != username)
                return Unauthorized("You are not authorized to delete this message");*/
            
            //or
            if (message.SenderUserName != username && message.RecipientUserName != username)
                return Unauthorized("You are not authorized to delete this message");


            if (message.SenderUserName ==username) message.SenderDeleted = true;

            if (message.RecipientUserName == username) message.RecipientDeleted = true;

            // Delete the message
            if (message.SenderDeleted && message.RecipientDeleted) {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }
            
            

            // Save changes to the database
            if (await _unitOfWork.Complete())
                return Ok();

            // Return a bad request if the message couldn't be deleted
            return BadRequest("Failed to delete the message");
        }


        [HttpDelete("delete-conversation/{recepientUsername}")]
        public async Task<IActionResult> DeleteConversationForUser(string recepientUsername)
        {
            if (string.IsNullOrEmpty(recepientUsername)) return BadRequest("Recipient username is required");

            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username)) return BadRequest("Failed to get the current user's username");

            await _unitOfWork.MessageRepository.MarkMessagesAsDeletedForUserAsync(username, recepientUsername.ToLower());
            var result = await _unitOfWork.Complete();
            _logger.LogInformation($"UnitOfWork.Complete() result: {result}");

            if (result) return NoContent();

            return BadRequest("Failed to delete the conversation");
        }








    }
}
