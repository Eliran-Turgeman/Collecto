using EmailCollector.Api.Areas.Identity.Data;
using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmailCollector.Api.Data;

public class EmailCollectorApiContext : IdentityDbContext<EmailCollectorApiUser>
{
    public EmailCollectorApiContext(DbContextOptions<EmailCollectorApiContext> options)
        : base(options)
    {
    }

    public DbSet<EmailSignup> EmailSignups { get; set; }
    public DbSet<SignupForm> SignupForms { get; set; }
    public DbSet<FormCorsSettings> FormCorsSettings { get; set; }
    public DbSet<SmtpEmailSettings> SmtpEmailSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SignupForm>()
            .Property(f => f.Status)
            .HasConversion(
            v => v.ToString(), // Convert enum to string when saving
            v => (FormStatus)Enum.Parse(typeof(FormStatus), v));  // Convert string to enum when loading


        modelBuilder.Entity<FormEmailSettings>()
            .HasKey(c => c.FormId);

        modelBuilder.Entity<SignupForm>()
            .HasOne(f => f.FormEmailSettings)
            .WithOne(e => e.Form)
            .HasForeignKey<FormEmailSettings>(e => e.FormId);

        modelBuilder.Entity<FormCorsSettings>()
            .HasKey(c => c.FormId);

        modelBuilder.Entity<SignupForm>()
            .HasOne(f => f.FormCorsSettings)
            .WithOne(c => c.Form)
            .HasForeignKey<FormCorsSettings>(c => c.FormId);

        // Configure table-per-concrete class (TPC) for email settings
        modelBuilder.Entity<SmtpEmailSettings>().ToTable("SmtpEmailSettings");

        modelBuilder.Entity<SmtpEmailSettings>()
            .Property(f => f.EmailMethod)
            .HasConversion(
            v => v.ToString(), // Convert enum to string when saving
            v => (EmailMethod)Enum.Parse(typeof(EmailMethod), v));  // Convert string to enum when loading

        base.OnModelCreating(modelBuilder);
    }
}
