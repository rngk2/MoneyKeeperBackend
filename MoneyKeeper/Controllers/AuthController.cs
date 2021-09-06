using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeepeer.Authentication.Models;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Attributes;
using MoneyKeeper.Authentication.Services;
using MoneyKeeper.Providers;

namespace MoneyKeeper.Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
        private readonly IAuthService authService;
        private readonly ICurrentUserProvider currentUserProvider;

        public AuthController(IAuthService authService, ICurrentUserProvider currentUserProvider)
        {
            this.authService = authService;
            this.currentUserProvider = currentUserProvider;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ApiResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
        {
            var (response, error) = await authService.Authenticate(model, currentUserProvider.GetCurrentUserIp()).Unwrap();

            if (error)
            {
                return error.Wrap();
            }

            SetTokenCookie(response.RefreshToken);
            return response;
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ApiResult<RefreshTokenResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var (response, error) = await authService.GetNewAccessToken(refreshToken).Unwrap();
            RefreshTokenResponse rtResponse = response;

            return error
                ? error.Wrap()
                : rtResponse;
        }

        /* 
         [HttpPost("revoke-token")]
		 public IActionResult RevokeToken(RevokeTokenRequest model)
		 {
			 // accept refresh token in request body or cookie
			 var token = model.Token ?? Request.Cookies["refreshToken"];

			 if (string.IsNullOrEmpty(token))
				 return BadRequest(new { message = "Token is required" });

			 _userService.RevokeToken(token, ipAddress());
			 return Ok(new { message = "Token revoked" });
		 }
        */

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
    }
}
