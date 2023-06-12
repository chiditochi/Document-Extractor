using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Document_Extractor.Models.DB
{
    public class AppUserDTO
    {
        public AppUserDTO()
        {
            LastLogin = DateTime.Now;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }

        public long UserTypeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [NotMapped]
        public string? Password { get; set; }

        // public virtual ICollection<AppRole> UserRoles { get; set; } = new List<AppRole>();
        public virtual UserTypeDTO UserType { get; set; } = new UserTypeDTO();

    }
}
