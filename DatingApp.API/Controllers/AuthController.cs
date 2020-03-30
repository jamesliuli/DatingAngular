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
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController: ControllerBase
    {
        private readonly IConfiguration _config;

        public DataContext _context { get; }
        public IMapper _mapper { get; }
        public UserManager<User> _userManager { get; }

        public SignInManager<User> _signInManager {get;}

        public AuthController(DataContext context,
                IConfiguration config, IMapper mapper, 
                UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
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

            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserDetailedDto>(userToCreate);
            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { controller = "Users", 
                    id = userToCreate.Id }, userToReturn);                            
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        //post URL http:5000/api/auth/login
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // missed photoUrl here, before use _context.Users.Include(p => p.Photos)
            var user = await _userManager.FindByNameAsync(userForLoginDto.username);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.password, false);
                if (result.Succeeded)
                {
                    // get photo
                    var userFromRepo = await _context.Users.Include(p=>p.Photos).FirstOrDefaultAsync( x=> x.UserName == user.UserName);
                    var appUser = _mapper.Map<UserForListDto>(userFromRepo);
                    return Ok( new {
                        token = GenerateJwtToken(user).Result,
                        user = appUser
                    });

                }
            }
            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
            return tokenHandler.WriteToken(token);
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