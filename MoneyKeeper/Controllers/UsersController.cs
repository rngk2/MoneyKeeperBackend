using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Authenticate.Models;
using Authenticate.Services;
using BL.Dtos.User;
using BL.Extensions;
using BL.Services;
using DAL.Models;
using Globals.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Attributes;

namespace MoneyKeeper.Controllers
{
	[Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IUserAuthService authService;

        public UsersController(IUserService userService, IUserAuthService authService)
        {
            this.userService = userService;
            this.authService = authService;
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
                if (e.Number == ((int)SqlErrorCodes.DUBLICATE_KEY_ERROR))  
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
                if (e.Number == ((int)SqlErrorCodes.DUBLICATE_KEY_ERROR))  
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

        [HttpGet("{id}/summary")]
        public async Task<IEnumerable<SummaryUnit>> GetSummary_ForMonth(int id)
        {
            return await userService.GetSummaryForUser(id);
        }

        [HttpGet("{id}/total/month")]
        public async Task<Dictionary<string, decimal>> GetTotal_ForMonth(int id)
        {
            return await userService.GetTotalForUser_ForMonth(id);
        }
        
        [HttpGet("{id}/total/year")]
        public async Task<Dictionary<string, decimal>> GetTotal_ForYear(int id)
        {
            return await userService.GetTotalForUser_ForYear(id);
        }


        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            var response = await authService.Authenticate(model, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

		[AllowAnonymous]
		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			var response = await authService.GetNewAccessToken(refreshToken);
			return new JsonResult(new { jwtToken = response });
		}

		/* [HttpPost("revoke-token")]
		 public IActionResult RevokeToken(RevokeTokenRequest model)
		 {
			 // accept refresh token in request body or cookie
			 var token = model.Token ?? Request.Cookies["refreshToken"];

			 if (string.IsNullOrEmpty(token))
				 return BadRequest(new { message = "Token is required" });

			 _userService.RevokeToken(token, ipAddress());
			 return Ok(new { message = "Token revoked" });
		 }*/

		/*[HttpGet]
		public IActionResult GetAll()
		{
			var users = _userService.GetAll();
			return Ok(users);

			return Ok();
		}*/

		/*  [HttpGet("{id}/refresh-tokens")]
          public async Task<IActionResult> GetRefreshTokens(int id)
          {
              var user = await _userService.GetById(id);
              return Ok(_userService.Get);
              return Ok();
          }*/

		// helper methods

		private void SetTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Lax,
                Secure = true
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }
}
