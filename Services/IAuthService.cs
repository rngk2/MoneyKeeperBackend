﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MoneyKeeper.Services
{
	public interface IAuthService
	{
		Task<JsonResult> GetUserWithToken(AuthService.UserCredentials credentials);
	}
}