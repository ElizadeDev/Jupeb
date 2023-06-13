using JupebPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace JupebPortal.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationForm>()
                .HasOne(a => a.Programme1)
                .WithMany()
                .HasForeignKey(a => a.Programme1Id)
                .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ApplicationForm>()
            .HasOne(a => a.Programme2)
            .WithMany()
            .HasForeignKey(a => a.Programme2Id)
            .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<ApplicationUser> User { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ApplicationForm> ApplicationForms { get; set; }
    public DbSet<Programme> Programmes { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<ApplicantOLevel> ApplicantOLevels { get; set; }
}
