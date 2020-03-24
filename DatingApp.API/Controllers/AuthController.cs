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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController: ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public IMapper _mapper { get; }
        public UserManager<User> _userManager { get; }

        public SignInManager<User> _signInManager {get;}

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper, 
                UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
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

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("user already exists");
            }

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var userCreated =  await _repo.Register(userToCreate, userForRegisterDto.Password);
            
            var userToReturn = _mapper.Map<UserDetailedDto>(userCreated);
            //return StatusCode(201);
            return CreatedAtRoute("GetUser", new { controller = "Users", 
                id = userCreated.Id }, userToReturn);                            
        }

        [HttpPost("login")]
        //post URL http:5000/api/auth/login
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // throw new Exception("Computer say no!");
            var userFromRepo = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);
            if (userFromRepo == null)
                return Unauthorized();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };

            var roles = await _userManager.GetClaimsAsync(userFromRepo);
            foreach (var role in roles) {
                claims.Add(role);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var user = _mapper.Map<UserForListDto>(userFromRepo);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok( new {
                token = tokenHandler.WriteToken(token),
                user
            });
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