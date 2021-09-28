using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.Authentication.Services;
using MoneyKeeper.Globals.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Utils.Results;
using MoneyKeeper.Api.Providers;
using MoneyKeeper.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using MoneyKeeper.BL.Services;
using MoneyKeeper.BL.Dtos.User;

namespace MoneyKeeper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthService authService;
        private readonly ICurrentUserProvider currentUserProvider;

        public UsersController(IUserService userService, IAuthService authService, ICurrentUserProvider currentUserProvider)
        {
            this.userService = userService;
            this.authService = authService;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet]
        public async Task<ApiResult<User>> GetUser()
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (user, service_error) = await userService.GetUser(contextUser.Id).Unwrap();

            return service_error is not null
                ? service_error.Wrap()
                : user;
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResult<User>> CreateUser(CreateUser userDto)
        {
            var (user, error) = await userService.CreateUser(userDto).Unwrap();
            return error 
                ? error.Wrap() 
                : user;
        }

        [HttpPut]
        public async Task<ApiResult<User>> UpdateUser(UpdateUser userDto)
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (updatedUser, service_error) = await userService.UpdateUser(contextUser.Id, userDto).Unwrap();

            return service_error
                ? service_error.Wrap()
                : updatedUser;
        }

        [HttpDelete]
        public async Task<ApiResult<global::MoneyKeeper.BL.Dtos.User.User>> DeleteUser()
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (deleted, service_error) = await userService.DeleteUser(contextUser.Id).Unwrap();

            return service_error
                ? service_error.Wrap()
                : deleted;
        }
    }
}
