using System.ComponentModel.DataAnnotations;

namespace MailSender.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public required string To { get; set; }
        
        public string? Cc { get; set; }
        
        public string? Bcc { get; set; }
        
        [Required]
        public required string Subject { get; set; }
        
        [Required]
        public required string Message { get; set; }
    }
}