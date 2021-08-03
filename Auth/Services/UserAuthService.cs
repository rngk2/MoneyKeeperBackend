using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Helpers;
using Auth.Models;
using Auth.Repositories;
using Microsoft.Extensions.Options;

namespace Auth.Services
{
    public interface IUserAuthService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
    //  void RevokeToken(string token, string ipAddress);
    //  IEnumerable<User> GetAll();
        Task<User> GetById(int id);
    }

    public class UserAuthService : IUserAuthService
    {
		private IUsersRepository repository;
		private IJwtUtils jwtUtils;
        private readonly AppSettings appSettings;

		public UserAuthService(
            IUsersRepository repository,
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings)
        {
            this.repository = repository;
            this.jwtUtils = jwtUtils;
            this.appSettings = appSettings.Value;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = await repository.GetUser(model.Email);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                throw new Exception("Username or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = jwtUtils.GenerateJwtToken(user);
            var refreshToken = jwtUtils.GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.Id;

            await repository.AddRefreshToken(refreshToken);



            // remove old refresh tokens from user
           // removeOldRefreshTokens(user);


            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = await repository.GetToken(token);

			/* if (refreshToken.IsRevoked)
			 {
				 // revoke all descendant tokens in case this token has been compromised
				 revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
				 _repo.Update(user);
				 _repo.SaveChanges();
			 }
 */
			/*if (!refreshToken.IsActive)
				throw new Exception("Invalid token");
*/
			// replace old refresh token with a new one (rotate token)
			var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
			await repository.AddRefreshToken(newRefreshToken);
            // remove old refresh tokens from user
            //await repository.RemoveOld();


			// generate new jwt
			var jwtToken = jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
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
		}*/

     /*   public IEnumerable<User> GetAll()
        {
            return _repo.Users;
        }*/

        public async Task<User> GetById(int id)
        {
            var user = await repository.GetUser(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        // helper methods
        private async Task<User> GetUserByRefreshToken(string token)
        {
            var user = await repository.GetUserByRefreshToken(token);

			if (user == null)
				throw new Exception("Invalid token");

			return user;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = jwtUtils.GenerateRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
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

		private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }



    }
}
