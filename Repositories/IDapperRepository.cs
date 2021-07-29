using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoneyKeeper.Repositories
{
	public interface IDapperRepository
	{
		Task<int> QuerySingle<T>(string sql, object @params = null);

		Task<IEnumerable<T>> QueryAny<T>(string sql, object @params = null);
		
		Task ExecuteAny<T>(string sql, object @params = null);

	} 
}