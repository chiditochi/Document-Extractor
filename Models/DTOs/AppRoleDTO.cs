using Microsoft.AspNetCore.Identity;

namespace Document_Extractor.Models.DB
{
    public class AppRoleDTO
    {
        public AppRoleDTO()
        {
            Label = string.Empty;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }
        public long UserTypeId { get; set; }
        public string Label { get; set; } //can be used to modify display
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual UserTypeDTO UserType { get; set; } = new UserTypeDTO();
    }
}
