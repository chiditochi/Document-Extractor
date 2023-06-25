using System.ComponentModel.DataAnnotations;

namespace Document_Extractor.Models.DB
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string? Password { get; set; }

    }
}
