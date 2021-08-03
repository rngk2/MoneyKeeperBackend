using System;
using System.Linq;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Authenticate
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
	{
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = context.HttpContext.Items["User"] as User;
            if (user == null)
            {
                //context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
	}
}
