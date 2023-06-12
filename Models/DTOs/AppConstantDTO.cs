namespace Document_Extractor.Models.DB
{
    public class AppConstantDTO
    {
        public AppConstantDTO()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }

        public long AppConstantId { get; set; }
        public string? Label { get; set; }
        public string? LabelValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
