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
				? new Error(ApiResultErrorCodes.USER_IS_MISSING.ToString(), "No user provided in HttpContext")
				: user;
		}
	}
}
