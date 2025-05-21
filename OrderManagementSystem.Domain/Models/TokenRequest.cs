using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Domain.Models
{
    public class TokenRequest
    {
        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string AppSecret { get; set; }
    }
} 