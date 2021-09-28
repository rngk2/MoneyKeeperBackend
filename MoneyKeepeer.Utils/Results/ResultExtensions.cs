using System.Diagnostics;
using System.Threading.Tasks;

namespace MoneyKeeper.Utils.Results
{
    public static class ResultExtensions
    {
        public static Error Wrap(this IError? error)
        {
            Debug.Assert(error != null, nameof(error) + " != null");
            return new Error(error);
        }

        public static async Task<UnwrapResult<TValue>> Unwrap<TValue>(this Task<Result<TValue>> task)
        {
            var result = await task;
            return result.Unwrap();
        }
    }
}