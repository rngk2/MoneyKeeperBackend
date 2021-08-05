using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authenticate.Helpers;
using Authenticate.Models;
using Authentication.Models;
using BCrypt.Net;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.Extensions.Options;

namespace Authenticate.Services
{
    public interface IUserAuthService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
        Task<string> GetNewAccessToken(string token);
    //  void RevokeToken(string token, string ipAddress);
    //  IEnumerable<User> GetAll();
        Task<User> GetById(int id);
    }

    public class UserAuthService : IUserAuthService
    {
        private readonly ITokensRepository tokensRepository;
        private readonly IUsersRepository usersRepository;
        private readonly AuthSettings appSettings;

		private IJwtUtils jwtUtils;
		
		public UserAuthService(
            IUsersRepository usersRepository,
            ITokensRepository tokensRepository,
            IOptions<AuthSettings> appSettings,
            IJwtUtils jwtUtils)
        {
            this.usersRepository = usersRepository;
            this.tokensRepository = tokensRepository;
            this.appSettings = appSettings.Value;
            this.jwtUtils = jwtUtils;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = await usersRepository.GetUser(model.Email);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                throw new Exception("Username or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = jwtUtils.GenerateJwtToken();
            var refreshToken = jwtUtils.GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.Id;

            await tokensRepository.AddRefreshToken(refreshToken);

            // remove old refresh tokens from user
            await tokensRepository.RemoveOldRefreshTokensOf(user.Id);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

		public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = await tokensRepository.GetToken(token);

			if (!refreshToken.IsActive)
				throw new Exception("Invalid token");

			// replace old refresh token with a new one (rotate token)
			var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
			await tokensRepository.AddRefreshToken(newRefreshToken);

            // remove old refresh tokens from user
            await tokensRepository.RemoveOldRefreshTokensOf(user.Id);

			// generate new jwt
			var jwtToken = jwtUtils.GenerateJwtToken();

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task<string> GetNewAccessToken(string token)
        {
            var refrshToken = await tokensRepository.GetToken(token);

            if (!refrshToken.IsActive)
            {
                throw new Exception("Invalid token");
            }

            return jwtUtils.GenerateJwtToken();

        }

      /*  public async Task RevokeToken(string token, string ipAddress)
        {
            var user = await getUserByRefreshToken(token);
            var refreshToken = await _repo.GetToken(token);

			if (!refreshToken.IsActive)
				throw new Exception("Invalid token");

			// revoke token and save
			revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
			_repo.Update(user);
			_repo.SaveChanges();
		}
      */

     /* public IEnumerable<User> GetAll()
        {
            return _repo.Users;
        }
     */

        public async Task<User> GetById(int id)
        {
            var user = await usersRepository.GetUser(id);

            if (user == null) 
                throw new KeyNotFoundException("User not found");
            
            return user;
        }

        // helper methods
        private async Task<User> GetUserByRefreshToken(string token)
        {
            var user = await tokensRepository.GetUserByRefreshToken(token);

			if (user == null)
				throw new Exception("Invalid token");

			return user;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = jwtUtils.GenerateRefreshToken(ipAddress);

            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            
            return newRefreshToken;
        }


		/* private void revokeDescendantRefreshTokens(RefreshToken refreshToken, User user, string ipAddress, string reason)
		 {
			 // recursively traverse the refresh token chain and ensure all descendants are revoked
			 if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
			 {
				 var childToken = user.RTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
				 if (childToken.IsActive)
					 revokeRefreshToken(childToken, ipAddress, reason);
				 else
					 revokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
			 }
		 }*/

		private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }



    }
}
