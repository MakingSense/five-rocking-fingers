namespace FRF.Core.Response
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public Error Error { get; set; }
        public T Value { get; set; }
    }
}
