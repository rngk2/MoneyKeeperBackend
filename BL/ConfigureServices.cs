using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MoneyKeeper.BL
{
	public static class ConfigureServices
	{
		public static void ConfigureApiServices(this IServiceCollection services)
		{
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<ICategoryService, CategoryService>();
			services.AddTransient<ITransactionService, TransactionService>();
		}
	}
}
