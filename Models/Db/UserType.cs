namespace Document_Extractor.Models.DB
{
    public class UserType
    {
        public UserType()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public long UserTypeId { get; set; }
        public string Label { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
