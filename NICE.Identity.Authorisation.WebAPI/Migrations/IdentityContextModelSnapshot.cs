﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NICE.Identity.Authorisation.WebAPI.Repositories;

namespace NICE.Identity.Authorisation.WebAPI.Migrations
{
    [DbContext(typeof(IdentityContext))]
    partial class IdentityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Environment", b =>
                {
                    b.Property<int>("EnvironmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("EnvironmentID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("EnvironmentId");

                    b.ToTable("Environments");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("JobID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsLead")
                        .HasColumnType("bit");

                    b.Property<int>("OrganisationId")
                        .HasColumnName("OrganisationID")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.HasKey("JobId");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("UserId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Organisation", b =>
                {
                    b.Property<int>("OrganisationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OrganisationID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("OrganisationId");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.OrganisationRole", b =>
                {
                    b.Property<int>("OrganisationRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("OrganisationRoleID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrganisationId")
                        .HasColumnName("OrganisationID")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnName("RoleID")
                        .HasColumnType("int");

                    b.HasKey("OrganisationRoleId");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("RoleId");

                    b.ToTable("OrganisationRoles");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RoleID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<int>("WebsiteId")
                        .HasColumnName("WebsiteID")
                        .HasColumnType("int");

                    b.HasKey("RoleId");

                    b.HasIndex("WebsiteId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Service", b =>
                {
                    b.Property<int>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ServiceID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("ServiceId");

                    b.ToTable("Services");

                    b.HasData(
                        new
                        {
                            ServiceId = 1,
                            Name = "NICE Website"
                        },
                        new
                        {
                            ServiceId = 2,
                            Name = "EPPI Reviewer v5"
                        });
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.TermsVersion", b =>
                {
                    b.Property<int>("TermsVersionId")
                        .HasColumnName("TermsVersionID")
                        .HasColumnType("int");

                    b.Property<int?>("CreatedByUserId")
                        .HasColumnName("CreatedByUserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("VersionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TermsVersionId");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("TermsVersions");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AllowContactMe")
                        .HasColumnType("bit");

                    b.Property<bool>("DormantAccountWarningSent")
                        .HasColumnType("bit");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(320)")
                        .HasMaxLength(320);

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<bool>("HasVerifiedEmailAddress")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("InitialRegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsInAuthenticationProvider")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLockedOut")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMigrated")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStaffMember")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLoggedInDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("NameIdentifier")
                        .IsRequired()
                        .HasColumnName("NameIdentifier")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.UserAcceptedTermsVersion", b =>
                {
                    b.Property<int>("UserAcceptedTermsVersionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserAcceptedTermsVersionID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TermsVersionId")
                        .HasColumnName("TermsVersionID")
                        .HasColumnType("int");

                    b.Property<DateTime>("UserAcceptedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserAcceptedTermsVersionId");

                    b.HasIndex("TermsVersionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAcceptedTermsVersion");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.UserEmailHistory", b =>
                {
                    b.Property<int>("UserEmailHistoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserEmailHistoryID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ArchivedByUserId")
                        .HasColumnName("ArchivedByUserID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ArchivedDateUTC")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(320)")
                        .HasMaxLength(320);

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.HasKey("UserEmailHistoryId");

                    b.HasIndex("ArchivedByUserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserEmailHistory");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.UserRole", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserRoleID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RoleId")
                        .HasColumnName("RoleID")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.HasKey("UserRoleId");

                    b.HasAlternateKey("UserId", "RoleId")
                        .HasName("IX_UserRoles_UserID_RoleID");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Website", b =>
                {
                    b.Property<int>("WebsiteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("WebsiteID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EnvironmentId")
                        .HasColumnName("EnvironmentID")
                        .HasColumnType("int");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<int>("ServiceId")
                        .HasColumnName("ServiceID")
                        .HasColumnType("int");

                    b.HasKey("WebsiteId");

                    b.HasIndex("EnvironmentId");

                    b.HasIndex("ServiceId");

                    b.ToTable("Websites");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Job", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.Organisation", "Organisation")
                        .WithMany("Jobs")
                        .HasForeignKey("OrganisationId")
                        .HasConstraintName("FK_Jobs_Organisation")
                        .IsRequired();

                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.User", "User")
                        .WithMany("Jobs")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Jobs_User")
                        .IsRequired();
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.OrganisationRole", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.Organisation", "Organisation")
                        .WithMany("OrganisationRoles")
                        .HasForeignKey("OrganisationId")
                        .HasConstraintName("FK_OrganisationRole_Organisation")
                        .IsRequired();

                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.Role", "Role")
                        .WithMany("OrganisationRoles")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_OrganisationRole_Role")
                        .IsRequired();
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Role", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.Website", "Website")
                        .WithMany("Roles")
                        .HasForeignKey("WebsiteId")
                        .HasConstraintName("FK_Roles_Roles")
                        .IsRequired();
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.TermsVersion", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.User", "CreatedByUser")
                        .WithMany("UserCreatedTermsVersions")
                        .HasForeignKey("CreatedByUserId")
                        .HasConstraintName("FK_TermsVersion_CreatedByUser");
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.UserAcceptedTermsVersion", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.TermsVersion", "TermsVersion")
                        .WithMany("UserAcceptedTermsVersions")
                        .HasForeignKey("TermsVersionId")
                        .HasConstraintName("FK_UserAcceptedTermsVersion_TermsVersion")
                        .IsRequired();

                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.User", "User")
                        .WithMany("UserAcceptedTermsVersions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserAcceptedTermsVersion_User")
                        .IsRequired();
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.UserEmailHistory", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.User", "ArchivedByUser")
                        .WithMany("ArchivedUserEmailHistory")
                        .HasForeignKey("ArchivedByUserId")
                        .HasConstraintName("FK_UserEmailHistory_ArchivedByUser_Users")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.User", "User")
                        .WithMany("UserEmailHistory")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserEmailHistory_User_Users")
                        .IsRequired();
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.UserRole", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRoles_Roles")
                        .IsRequired();

                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRoles_Users")
                        .IsRequired();
                });

            modelBuilder.Entity("NICE.Identity.Authorisation.WebAPI.DataModels.Website", b =>
                {
                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.Environment", "Environment")
                        .WithMany("Websites")
                        .HasForeignKey("EnvironmentId")
                        .HasConstraintName("FK_ServiceInstance_Environments")
                        .IsRequired();

                    b.HasOne("NICE.Identity.Authorisation.WebAPI.DataModels.Service", "Service")
                        .WithMany("Websites")
                        .HasForeignKey("ServiceId")
                        .HasConstraintName("FK_ServiceInstance_Services")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
