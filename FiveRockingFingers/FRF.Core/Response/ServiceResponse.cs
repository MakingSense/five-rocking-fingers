namespace FRF.Core.Response
{
    public class ServiceResponse<T>
    {
        public bool Success { get; }
        public Error Error { get; }
        public T Value { get; }

        public ServiceResponse(T value)
        {
            Success = true;
            Value = value;
        }

        public ServiceResponse(Error error)
        {
            Success = false;
            Error = error;
        }
    }
}
