namespace MoneyKeeper.Api.Results
{
    public interface IError
    {
        string Code { get; }
        string Message { get; }
    }
    
    public readonly ref struct Error
    {
        public Error(IError error)
        {
            Code = error.Code;
            Message = error.Message;
        }
        
        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; }
        public string Message { get; }
    }
}