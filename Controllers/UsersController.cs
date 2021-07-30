using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoneyKeeper.Dtos;
using MoneyKeeper.Entities;
using MoneyKeeper.Extensions;
using MoneyKeeper.Repositories;
using MoneyKeeper.Services;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService userService;

		public UsersController(IUserService userService)
		{
            this.userService = userService;
		}

        // GET /users
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            return (await userService.GetUsers()).Select(user => user.AsDto());
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await userService.GetUser(id);
            return user is null ? NotFound() : user.AsDto();
        }

        // POST /users
        [HttpPost] 
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userDto)
        {
            try
            {
                var createdUser = await userService.CreateUser(userDto);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (SqlException e)
            {
                if (e.Number == 2627) // dublicate key error number
                {
                    return new ConflictObjectResult($"User with email={userDto.Email} already exist");
                }

                return new StatusCodeResult(500);
            }
        }

        // PUT /users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UpdateUserDto userDto)
        {
            var existingUser = await userService.GetUser(id);

            if (existingUser is null)
            {
                return NotFound();
            }

            try
            {
                await userService.UpdateUser(existingUser, userDto);
                return NoContent();
            }
            catch (SqlException e)
            {
                if (e.Number == 2627) // dublicate key error number
                {
                    return new ConflictObjectResult($"User with email={userDto.Email} already exist");
                }

                return new StatusCodeResult(500);
            }
        }

		// DELETE /users/{id}
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
            await userService.DeleteUser(id);
            return NoContent();
		}


        [HttpPost("auth")]
        public async Task<ActionResult> Token(string email, string password)
        {
            var identity = await GetIdentityAsync(email, password);
            if (identity == null)
            {
                return BadRequest(new { error = "Invalid email or password." });
            }

            var now = DateTime.UtcNow;

            var jwtToken = new JwtSecurityToken(
                    issuer: JwtAuthOptions.ISSUER,
                    audience: JwtAuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(JwtAuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(JwtAuthOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var response = new
            {
                access_token = encodedJwt,
                email = identity.Claims.ElementAt(0).Value,
                id = identity.Claims.ElementAt(1).Value
            };

            return new JsonResult(response);
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(string email, string password)
        {
            User user = await userService.GetUser(email);
            if (user != null && user.Password.Equals(password.AsSHA256Hash()))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims, "Token", 
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType
                );
                
                return claimsIdentity;
            }

            // if user not found
            return null;
        }

    }
}
