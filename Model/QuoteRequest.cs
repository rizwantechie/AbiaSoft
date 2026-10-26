using System;
using System.ComponentModel.DataAnnotations;

namespace AbiaSoft.Model
{
    public class QuoteRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Service is required")]
        public string Service { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(500)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
