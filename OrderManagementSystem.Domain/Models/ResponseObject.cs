namespace OrderManagementSystem.Domain.Models
{
    public class ResponseObject<T>
    {
        public T[] Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; } = true;
    }
} 