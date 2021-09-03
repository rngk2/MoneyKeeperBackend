using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
	public interface IDapperRepository
	{
		Task<O> QuerySingleWithOutput<O>(string sql, object? @params = null);

		Task<IEnumerable<T>> QueryAny<T>(string sql, object? @params = null);
		
		Task<int> ExecuteAny(string sql, object? @params = null);

	} 
}