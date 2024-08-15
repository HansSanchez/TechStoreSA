namespace TechStoreSA.Shared
{
    public class ResponseAPI<T>
    {
        public bool IsSuccessful { get; set; }
        public T? Value { get; set; }
        public string? Message { get; set; }

    }
}
