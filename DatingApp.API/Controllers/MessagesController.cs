using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Helps;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{

    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        public IMapper _mapper { get; }
        public IDatingRepository _repo { get; }
        public MessagesController(IDatingRepository repo, IMapper mapper) {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo == null)
                return NotFound();

            var messageForReturn = _mapper.Map<MessageForReturnDto>(messageFromRepo);
            return Ok(messageForReturn);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messages = await _repo.GetMessagesForUser(messageParams);
            var messagesForReturn = _mapper.Map<IEnumerable<MessageForReturnDto>>(messages);
            return Ok(messagesForReturn);
        }

        // ERROR!!! -> [HttpGet("/thread/{recipientId}")]  (Extra '/')
        [HttpGet("thread/{recipientId}")]
         public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
         {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                 return Unauthorized();

              var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);

              var messageThread = _mapper.Map<IEnumerable<MessageForReturnDto>>(messagesFromRepo);

              return Ok(messageThread);
         }        

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            //often used mthod to check the current logged in user
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var senderFromRepo = await _repo.GetUser(userId);
            var receiverFromRepo = await _repo.GetUser(messageForCreationDto.RecipientId);
            if (senderFromRepo == null || receiverFromRepo == null)
                return BadRequest("Couldn't find recipient");

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.Add<Message>(message);            
            // senderFromRepo.MessagesSent.Add(message);
            // receiverFromRepo.MessagesReceived.Add(message);
            if(await _repo.SaveAll())
            {
                var messageForReturn = _mapper.Map<MessageForReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, messageForReturn);
            }

            return BadRequest("Failed to send message");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo == null)
                return NotFound();

            if (await _repo.DeleteMessage(userId, id))
            {
                return Ok();
            }

            return BadRequest("Delete message failed");
        }        
    }
}