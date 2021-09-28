namespace MoneyKeeper.Utils.Results
{
    public abstract class Result<TValue>
    {
        public static implicit operator Result<TValue>(TValue value)
            => new SuccessResult<TValue>(value);
        
        public static implicit operator Result<TValue>(Error error)
            => new ErrorResult<TValue>(error);

        public UnwrapResult<TValue> Unwrap()
        {
            if (this is ErrorResult<TValue> error)
            {
                return new(default, error);
            }

            var success = (SuccessResult<TValue>)this;
            return new(success.Value, null);
        }
    }

    public class SuccessResult<TValue> : Result<TValue>
    {
        public SuccessResult(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }
    }
    
    public class ErrorResult<TValue> : Result<TValue>, IError
    {
        public ErrorResult(Error error)
        {
            Code = error.Code;
            Message = error.Message;
        }
        
        public static implicit operator bool(ErrorResult<TValue>? errorResult) 
            => errorResult is not null;

        public string Code { get; set; }
        public string Message { get; set; }
    }
}