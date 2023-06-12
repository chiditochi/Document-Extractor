using Document_Extractor.Models.DB;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Document_Extractor.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Team> Teams { get; set; } = null!;
    public virtual DbSet<Patient> Patients { get; set; } = null!;
    public virtual DbSet<AppConstant> AppConstants { get; set; } = null!;
    public virtual DbSet<UserType> UserTypes { get; set; } = null!;
}
