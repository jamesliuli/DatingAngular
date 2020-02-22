using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IDatingRepository _repo;

        private IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var loginUser = await _repo.GetUser(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            var users = await _repo.GetUsers();
            users = users.Where( u => u.Gender != loginUser.Gender);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            usersToReturn = usersToReturn.Where( u => u.Age >= 18 && u.Age <= 99);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserDetailedDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            //often used mthod to check the current logged in user
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userInRepo = await _repo.GetUser(id);
            _mapper.Map(userForUpdateDto, userInRepo);
            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception($"Update user {id} failed");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            if (await _repo.GetLike(id, recipientId) != null)
            {
                return BadRequest("You already like this user");
            }

            if (await _repo.GetUser(recipientId) == null)
            {
                return NotFound();
            }

            var like = new Like {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to add like");
        }

        // //GetLiker
        [HttpGet("{id}/liker")]        
        public async Task<IActionResult> GetLikes(int id, bool liker)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repo.GetUser(id);
            
            var likes = await _repo.GetUserLikes(id, liker);
            var users = await _repo.GetUsers();
            users = users.Where( u => likes.Contains(u.Id));
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }
    }
}