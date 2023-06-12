namespace Document_Extractor.Models.DB
{
    public class Team
    {
        public Team()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }

        public long TeamId { get; set; }
        public string? Code { get; set; }
        public string? CodeDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
