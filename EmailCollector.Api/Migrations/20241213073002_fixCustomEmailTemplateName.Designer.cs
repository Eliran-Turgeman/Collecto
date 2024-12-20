﻿// <auto-generated />
using System;
using EmailCollector.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmailCollector.Api.Migrations
{
    [DbContext(typeof(EmailCollectorApiContext))]
    [Migration("20241213073002_fixCustomEmailTemplateName")]
    partial class fixCustomEmailTemplateName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("EmailCollector.Domain.Entities.ApiKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("INTEGER");

                    b.Property<string>("KeyHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.CustomEmailTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Event")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FormId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TemplateBodyUri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TemplateSubject")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("CustomEmailTemplates");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.EmailCollectorApiUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.EmailSignup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SignupDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("SignupFormId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EmailSignups");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.FormCorsSettings", b =>
                {
                    b.Property<Guid>("FormId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AllowedOrigins")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("FormId");

                    b.ToTable("FormCorsSettings");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.FormEmailSettings", b =>
                {
                    b.Property<Guid>("FormId")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailFrom")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailMethod")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("FormId");

                    b.ToTable("FormEmailSettings");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.RecaptchaFormSettings", b =>
                {
                    b.Property<Guid>("FormId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SecretKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("SiteKey")
                        .HasColumnType("TEXT");

                    b.HasKey("FormId");

                    b.ToTable("RecaptchaFormSettings");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.SignupForm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("FormName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SignupForms");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.SmtpEmailSettings", b =>
                {
                    b.HasBaseType("EmailCollector.Domain.Entities.FormEmailSettings");

                    b.Property<string>("SmtpPassword")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SmtpPort")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SmtpServer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SmtpUsername")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.ToTable("SmtpEmailSettings", (string)null);
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.ApiKey", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.EmailCollectorApiUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.CustomEmailTemplate", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.SignupForm", "Form")
                        .WithMany("CustomEmailTemplates")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.FormCorsSettings", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.SignupForm", "Form")
                        .WithOne("FormCorsSettings")
                        .HasForeignKey("EmailCollector.Domain.Entities.FormCorsSettings", "FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.FormEmailSettings", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.SignupForm", "Form")
                        .WithOne("FormEmailSettings")
                        .HasForeignKey("EmailCollector.Domain.Entities.FormEmailSettings", "FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.RecaptchaFormSettings", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.SignupForm", "Form")
                        .WithOne("RecaptchaSettings")
                        .HasForeignKey("EmailCollector.Domain.Entities.RecaptchaFormSettings", "FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.EmailCollectorApiUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.EmailCollectorApiUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EmailCollector.Domain.Entities.EmailCollectorApiUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.EmailCollectorApiUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.SmtpEmailSettings", b =>
                {
                    b.HasOne("EmailCollector.Domain.Entities.FormEmailSettings", null)
                        .WithOne()
                        .HasForeignKey("EmailCollector.Domain.Entities.SmtpEmailSettings", "FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EmailCollector.Domain.Entities.SignupForm", b =>
                {
                    b.Navigation("CustomEmailTemplates");

                    b.Navigation("FormCorsSettings")
                        .IsRequired();

                    b.Navigation("FormEmailSettings")
                        .IsRequired();

                    b.Navigation("RecaptchaSettings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
