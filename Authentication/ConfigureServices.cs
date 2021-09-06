using Microsoft.Extensions.DependencyInjection;
using MoneyKeeper.Authentication.Services;
using MoneyKeeper.Authentication.Utils;

namespace MoneyKeeper.Authentication
{
	public static class ConfigureServices
	{
		public static void ConfigureAuthServices(this IServiceCollection services)
		{
			services.AddTransient<IJwtUtils, JwtUtils>();
			services.AddTransient<IAuthService, AuthService>();
		}
	}
}
