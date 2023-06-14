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
    public class AppUser : IdentityUser<long>
    {
        public AppUser()
        {
            LastLogin = DateTime.Now;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

        }

        public long? UserTypeId { get; set; }
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
        public virtual UserType UserType { get; set; } = new UserType();

    }
}
