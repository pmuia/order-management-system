namespace OrderManagementSystem.Domain.Models
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public long Expires { get; set; }
        public string TokenType { get; set; }
    }
} 