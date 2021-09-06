using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Globals.Errors;

namespace MoneyKeeper.Providers
{
	public interface ICurrentUserProvider
	{
		Result<User> GetCurrentUser();

		string GetCurrentUserIp();
	}


	public class CurrentUserProvider : ICurrentUserProvider
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public Result<User> GetCurrentUser()
		{
			return httpContextAccessor?.HttpContext?.Items["User"] is not User user
				? new Error(ApiResultErrorCodes.USER_IS_MISSING, "No user provided in HttpContext")
				: user;
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
