using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Authenticate.Models;
using Authenticate.Services;
using Authentication.Models;
using BL.Dtos.User;
using BL.Extensions;
using BL.Services;
using DAL.Entities;
using DAL.Models;
using Globals.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Attributes;
using MoneyKeeper.Providers;

namespace MoneyKeeper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IUserAuthService authService;
        private readonly ICurrentUserProvider currentUserProvider;

        public UsersController(IUserService userService, IUserAuthService authService, ICurrentUserProvider currentUserProvider)
        {
            this.userService = userService;
            this.authService = authService;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet]
        public async Task<ApiResult<UserDto>> GetUser()
        {
            var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap(); 
            }

            var (user, service_error) = await userService.GetUser(contextUser.Id).Unwrap();
            
            return service_error is not null
                ? service_error.Wrap()
                : user.AsDto();
        }

        // POST /users
        [HttpPost]
        public async Task<ApiResult<UserDto>> CreateUser(CreateUserDto userDto)
        {
			var (user, error) = await userService.CreateUser(userDto).Unwrap();
            return error ? error.Wrap() : user.AsDto();
        }

        [HttpPut]
        public async Task<ApiResult<UserDto>> UpdateUser(UpdateUserDto userDto)
        {
            var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (updatedUser, service_error) = await userService.UpdateUser(contextUser.Id, userDto).Unwrap();

            return service_error
                ? service_error.Wrap()
                : updatedUser.AsDto();
        }

        [HttpDelete]
        public async Task<ApiResult<UserDto>> DeleteUser()
        {
            var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (deleted, service_error) = await userService.DeleteUser(contextUser.Id).Unwrap();

            return service_error
                ? service_error.Wrap()
                : deleted.AsDto();
        }

        [HttpGet("summary")]
        public async Task<ApiResult<IEnumerable<SummaryUnit>>> GetSummary_ForMonth()
        {
            var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            return (await userService.GetSummaryForUser(contextUser.Id).Unwrap()).Value!.ToList();
        }

        [HttpGet("total/month")]
        public async Task<ApiResult<Dictionary<string, decimal>>> GetTotal_ForMonth()
        {
            var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            return (await userService.GetTotalForUser_ForMonth(contextUser.Id).Unwrap()).Value!;
        }

        [HttpGet("total/year")]
        public async Task<ApiResult<Dictionary<string, decimal>>> GetTotal_ForYear()
        {
            var (contextUser, provider_error) = currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            return (await userService.GetTotalForUser_ForYear(contextUser.Id).Unwrap()).Value!;
        }


        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ApiResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
        {
            var response = await authService.Authenticate(model, IpAddress());
            
            SetTokenCookie(response.RefreshToken);
            
            return response;
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ApiResult<RefreshTokenResponse>> RefreshToken()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			var response = await authService.GetNewAccessToken(refreshToken);
            
            return new RefreshTokenResponse(response);
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
