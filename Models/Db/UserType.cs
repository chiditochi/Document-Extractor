namespace Document_Extractor.Models.DB
{
    public class UserType
    {
        public UserType()
        {
            Label = string.Empty;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public long UserTypeId { get; set; }
        public string Label { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
