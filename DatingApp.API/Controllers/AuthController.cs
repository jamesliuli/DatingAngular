using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        //api/auth/register
        [HttpPost("register")]
        //post URL http:5000/api/auth/register
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        //public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
        {
            //validate  if without [ApiCintroller] tag
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);

            userForRegisterDto.username = userForRegisterDto.username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.username))
            {
                return BadRequest("user already exists");
            }

            var userToCreate = new User {
                Username = userForRegisterDto.username
            };


            var userCreated =  await _repo.Register(userToCreate, userForRegisterDto.password);
            //return CreatedAtRoute()
            return StatusCode(201);
        }

        [HttpPost("login")]
        //post URL http:5000/api/auth/login
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // try
            // {
                // throw new Exception("Computer say no!");
                var userFromRepo = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);
                if (userFromRepo == null)
                    return Unauthorized();
                var claims = new []
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok( new {
                    token = tokenHandler.WriteToken(token)
                });

            // }
            // catch
            // {
            //     return StatusCode(500, "Computer server error!");
            // }
        }

        // [HttpGet]
        // public async Task<IActionResult> GetValues()
        // {
        //     var values = new List<Value>();
        //     values.Add(new Value() { Id = 1, Name = "123" });

        //     return Ok(values);
        // }
    }
}