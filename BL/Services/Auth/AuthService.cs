using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BL.Extensions;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace BL.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserService userService;

		public record UserCredentials
		{
			public string Email { get; init; }

			public string Password { get; init; }
		}

		public AuthService(IUserService userService)
		{
			this.userService = userService;
		}

		public async Task<JsonResult> GetUserWithToken(UserCredentials credentials)
		{
			var identity = await GetIdentityAsync(credentials.Email, credentials.Password);
			if (identity == null)
			{
				return new JsonResult(new { error = "Invalid email or password." });
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
				id = identity.Claims.ElementAt(0).Value,
				firstName = identity.Claims.ElementAt(1).Value,
				lastName = identity.Claims.ElementAt(2).Value,
				email = identity.Claims.ElementAt(3).Value,
				accessToken = encodedJwt
			};

			return new JsonResult(new { ok = response });
		}

		private async Task<ClaimsIdentity> GetIdentityAsync(string email, string password)
		{
			User user = await userService.GetUser(email);
			if (user != null && user.Password.HashEquals(password))
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.FirstName),
					new Claim(ClaimTypes.Name, user.LastName),
					new Claim(ClaimTypes.Name, user.Email)
				};

				ClaimsIdentity claimsIdentity = new(
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
