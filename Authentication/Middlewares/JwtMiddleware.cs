using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authenticate.Helpers;
using Authenticate.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Authenticate
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthSettings _appSettings;

        private static readonly string[] _skipRoutes = {"/authenticate"};  

        public JwtMiddleware(RequestDelegate next, IOptions<AuthSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserAuthService userService, IJwtUtils jwtUtils)
        {
            if (!context.Request.Path.Value.Contains(_skipRoutes.First()))
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var userId = jwtUtils.ValidateJwtToken(token);
                if (userId != null)
                {
                    // attach user to context on successful jwt validation
                    context.Items["User"] = await userService.GetById(userId.Value);
                }
            }

            await _next(context);
        }
    }
}
