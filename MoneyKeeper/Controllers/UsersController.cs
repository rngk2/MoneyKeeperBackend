using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MoneyKeeper.Authentication.Services;
using BL.Dtos.User;
using BL.Extensions;
using BL.Services;
using DAL.Entities;
using DAL.Models;
using Globals.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Api.Results;
using MoneyKeeper.Providers;
using MoneyKeepeer.Authentication.Models;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ApiResult<UserDto>> GetUser()
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (user, service_error) = await userService.GetUser(contextUser.Id).Unwrap();

            return service_error is not null
                ? service_error.Wrap()
                : user.AsDto();
        }

        [HttpPost]
        public async Task<ApiResult<UserDto>> CreateUser(CreateUserDto userDto)
        {
            var (user, error) = await userService.CreateUser(userDto).Unwrap();
            return error ? error.Wrap() : user.AsDto();
        }

        [HttpPut]
        public async Task<ApiResult<UserDto>> UpdateUser(UpdateUserDto userDto)
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (updatedUser, service_error) = await userService.UpdateUser(contextUser.Id, userDto).Unwrap();

            return service_error
                ? service_error.Wrap()
                : updatedUser.AsDto();
        }

        [HttpDelete]
        public async Task<ApiResult<UserDto>> DeleteUser()
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            var (deleted, service_error) = await userService.DeleteUser(contextUser.Id).Unwrap();

            return service_error
                ? service_error.Wrap()
                : deleted.AsDto();
        }

        [HttpGet("summary")]
        public async Task<ApiResult<IEnumerable<SummaryUnit>>> GetSummary_ForMonth()
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            { 
                return provider_error.Wrap();
            }

            return (await userService.GetSummaryForUser(contextUser.Id).Unwrap()).Value!.ToList();
        }

        [HttpGet("total/month")]
        public async Task<ApiResult<Dictionary<string, decimal>>> GetTotal_ForMonth()
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            return (await userService.GetTotalForUser_ForMonth(contextUser.Id).Unwrap()).Value!;
        }

        [HttpGet("total/year")]
        public async Task<ApiResult<Dictionary<string, decimal>>> GetTotal_ForYear()
        {
            var (contextUser, provider_error) = await currentUserProvider.GetCurrentUser().Unwrap();

            if (provider_error)
            {
                return provider_error.Wrap();
            }

            return (await userService.GetTotalForUser_ForYear(contextUser.Id).Unwrap()).Value!;
        }
    }
}
