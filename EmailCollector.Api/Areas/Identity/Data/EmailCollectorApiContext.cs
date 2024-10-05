using EmailCollector.Api.Areas.Identity.Data;
using EmailCollector.Domain.Entities;
using Microsoft.AspNetCore.Identity;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SignupForm>()
            .Property(f => f.Status)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);
    }
}
