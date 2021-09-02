namespace MoneyKeeper.Api.Results
{
    public class ApiResult<TValue>
    {
        public ApiResult(TValue? value, IError? error)
        {
            Value = value;
            Error = error;
        }

        public TValue? Value { get; }
        public IError? Error { get; }
        
        public static implicit operator ApiResult<TValue>(TValue value)
        {
            return new ApiResult<TValue>(value, null);
        }
        
        public static implicit operator ApiResult<TValue>(Error error)
        {
            return new ApiResult<TValue>(default, new ErrorResult<TValue>(error));
        }
    }
}