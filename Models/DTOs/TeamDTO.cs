using System.ComponentModel.DataAnnotations;

namespace Document_Extractor.Models.DB
{
    public class TeamDTO
    {
        public TeamDTO()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }

        public long TeamId { get; set; }
        [Required]
        public string? Code { get; set; }
        [Required]
        public string? CodeDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
