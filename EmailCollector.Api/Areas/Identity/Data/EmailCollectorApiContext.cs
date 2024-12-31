using EmailCollector.Domain.Entities;
using EmailCollector.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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
    public DbSet<ApiKey> ApiKeys { get; set; }
    
    public DbSet<RecaptchaFormSettings> RecaptchaFormSettings { get; set; }
    
    public DbSet<CustomEmailTemplate> CustomEmailTemplates { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SignupForm>()
            .Property(f => f.Status)
            .HasConversion(
            v => v.ToString(), // Convert enum to string when saving
            v => (FormStatus)Enum.Parse(typeof(FormStatus), v));  // Convert string to enum when loading
        
        modelBuilder.Entity<SignupForm>()
            .Navigation(f => f.FormCorsSettings)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<FormEmailSettings>()
            .HasKey(c => c.FormId);

        modelBuilder.Entity<SignupForm>()
            .HasOne(f => f.FormEmailSettings)
            .WithOne(e => e.Form)
            .HasForeignKey<FormEmailSettings>(e => e.FormId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormCorsSettings>()
            .HasKey(c => c.FormId);

        modelBuilder.Entity<SignupForm>()
            .HasOne(f => f.FormCorsSettings)
            .WithOne(c => c.Form)
            .HasForeignKey<FormCorsSettings>(c => c.FormId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure table-per-concrete class (TPC) for email settings
        modelBuilder.Entity<SmtpEmailSettings>().ToTable("SmtpEmailSettings");

        modelBuilder.Entity<SmtpEmailSettings>()
            .Property(f => f.EmailMethod)
            .HasConversion(
            v => v.ToString(), // Convert enum to string when saving
            v => (EmailMethod)Enum.Parse(typeof(EmailMethod), v));  // Convert string to enum when loading
        
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.KeyHash).IsRequired();

            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany() // No navigation property in ApplicationUser
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<RecaptchaFormSettings>()
            .HasKey(r => r.FormId);
        
        modelBuilder.Entity<SignupForm>()
            .HasOne(f => f.RecaptchaSettings)
            .WithOne(c => c.Form)
            .HasForeignKey<RecaptchaFormSettings>(c => c.FormId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SignupForm>()
            .HasMany(s => s.CustomEmailTemplates)
            .WithOne(et => et.Form)
            .HasForeignKey(et => et.FormId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<CustomEmailTemplate>()
            .Property(f => f.Event)
            .HasConversion(
                v => v.ToString(), // Convert enum to string when saving
                v => (TemplateEvent)Enum.Parse(typeof(TemplateEvent), v));  // Convert string to enum when loading
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
    }
}
