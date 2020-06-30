using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp_API.Data;
using DatingApp_API.DTOs;
using DatingApp_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO)
        {
            // capitalization should not affect usernames
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();
            
            if(await _repo.UserExists(userForRegisterDTO.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDTO.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDTO.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            // Check if we have a user that matches db, username stored in lowercase
            var userFromRepo = await _repo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);
            // if interface returned null, user does not excist. Therefore only return Unauthorized to don't show more than needed
            if (userFromRepo == null)
                return Unauthorized();

            //Creating two claims for the JSON web token (JWT), claims of user ID and username
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //To make sure the token is valid on return, the server creates a signature 
            //First create a key from random string in AppSettings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //Encrypt the key with a hashing alg
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Start to create the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //claims are passed into the subject of the token
                Subject = new ClaimsIdentity(claims),
                //expires in 24h
                Expires = DateTime.Now.AddDays(1),
                //sign with hashed credentials
                SigningCredentials = creds
            };

            //Create handler that will create token from tokenDescriptor
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            //return token to client, written into respons 
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });

            //jwt.io to decode tokens           
        }
    }
}