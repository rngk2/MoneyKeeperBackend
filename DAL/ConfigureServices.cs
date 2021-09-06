using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace MoneyKeeper.DAL
{
	public static class ServiceCollectionExtensions
	{
		public static void ConfigureRepos(this IServiceCollection services)
		{
			services.AddSingleton<IDapperRepository, DapperRepository>();
			services.AddSingleton<IUsersRepository, DapperUsersRepository>();
			services.AddSingleton<ITokensRepository, DapperTokensRepository>();
			services.AddSingleton<ICategoriesRepository, DapperCategoriesRepository>();
			services.AddSingleton<ITransactionsRepository, DapperTransactionsRepository>();
		}
	}
}
