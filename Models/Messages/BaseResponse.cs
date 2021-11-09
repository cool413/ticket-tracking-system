namespace Models.Messages
{
    public sealed class BaseResponse<T>
    {
        public int StatusCode { get; set; } = 200;

        public string Msg { get; set; } = string.Empty;

        public T Result { get; set; }
    }
}
