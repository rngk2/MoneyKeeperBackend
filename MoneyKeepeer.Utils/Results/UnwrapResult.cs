namespace MoneyKeeper.Api.Results
{
    public class UnwrapResult<TValue>
    {
        public UnwrapResult(TValue? value, ErrorResult<TValue>? error)
        {
            Value = value;
            Error = error;
        }

        public TValue? Value { get; }

        public ErrorResult<TValue>? Error { get; }

        public void Deconstruct(out TValue value, out ErrorResult<TValue>? error)
        {
            value = Value!;
            error = Error;
        }
    }
}