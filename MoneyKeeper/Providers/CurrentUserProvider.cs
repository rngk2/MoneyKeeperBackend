using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using Microsoft.AspNetCore.Http;

namespace MoneyKeeper.Providers
{
	public interface ICurrentUserProvider
	{
		User GetCurrentUser();
	}


	public class CurrentUserProvider : ICurrentUserProvider
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public User GetCurrentUser()
		{
			return httpContextAccessor.HttpContext.Items["User"] as User;
		}
	}
}
