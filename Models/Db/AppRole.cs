using Microsoft.AspNetCore.Identity;

namespace Document_Extractor.Models.DB
{
    public class AppRole : IdentityRole<long>
    {
        public AppRole()
        {
            Label = string.Empty;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }
        public long UserTypeId { get; set; }
        public string Label { get; set; } //can be used to modify display
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual UserType UserType { get; set; } = new UserType();
    }
}
