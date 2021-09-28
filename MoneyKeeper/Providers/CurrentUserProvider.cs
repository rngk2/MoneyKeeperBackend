using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MoneyKeeper.Utils.Results;
using MoneyKeeper.Authentication.Services;
using MoneyKeeper.Globals.Errors;
using MoneyKeeper.BL.Dtos.User;

namespace MoneyKeeper.Api.Providers
{
	public interface ICurrentUserProvider
	{
		Task<Result<User>> GetCurrentUser();

		string GetCurrentUserIp();
	}


	public class CurrentUserProvider : ICurrentUserProvider
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ClaimsPrincipal? userClaims;
		
		private readonly IAuthService authService;

		public CurrentUserProvider(IHttpContextAccessor httpContextAccessor, IAuthService authService)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.authService = authService;

			userClaims = httpContextAccessor.HttpContext?.User;
		}

		public async Task<Result<User>> GetCurrentUser()
		{
			if (userClaims is null)
			{
				return new Error(ApiResultErrorCodes.USER_IS_MISSING, "No user provided in HttpContext");
			}

			int userId = int.Parse(userClaims.Claims.Where(c => c.Type == "id").First().Value);

			var (user, service_error) = await authService.GetById(userId).Unwrap();

			if (service_error is not null) 
			{
				throw new Exception(service_error.Code + ": " + service_error.Message);
			}

			return user;
		}

        public string GetCurrentUserIp()
        {
			if (httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Forwarded-For") is not null)
			{
				return httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString();
			}
			
			return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString()!;
		}
    }
}
