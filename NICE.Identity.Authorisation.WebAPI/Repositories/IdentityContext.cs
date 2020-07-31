using Microsoft.EntityFrameworkCore;
using NICE.Identity.Authorisation.WebAPI.DataModels;

namespace NICE.Identity.Authorisation.WebAPI.Repositories
{
    public partial class IdentityContext : DbContext
    {
        public IdentityContext()
        {
        }

        public IdentityContext(DbContextOptions<Repositories.IdentityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Environment> Environments { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Website> Websites { get; set; }
        public virtual DbSet<TermsVersion> TermsVersions { get; set; }
        public virtual DbSet<UserAcceptedTermsVersion> UserAcceptedTermsVersions { get; set; }
        public virtual DbSet<Organisation> Organisations { get; set; }
        public virtual DbSet<OrganisationRole> OrganisationRoles { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("[you don't need a valid connection string when creating migrations. the real connection string should never be put here though. it should be kept in secrets.json]");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Environment>(entity =>
            {
                entity.ToTable("Environments");

                entity.HasKey(e => e.EnvironmentId);

                entity.Property(e => e.EnvironmentId).HasColumnName("EnvironmentID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasData(new Environment[] {
                    new Environment(1, "Local", 10),
                    new Environment(2, "Dev", 20),
                    new Environment(3, "Test", 30),
                    new Environment(4, "Alpha", 40),
                    new Environment(5, "Beta", 50),
                    new Environment(6, "Live", 60),
                });
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");

                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.WebsiteId).HasColumnName("WebsiteID");

                entity.HasOne(d => d.Website)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.WebsiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Roles_Roles");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Services");

                entity.HasKey(e => e.ServiceId);

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasData(new Service[] {
                    new Service(1, "NICE Website"),
                    new Service(2, "EPPI Reviewer v5")
                });
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");

                entity.HasKey(e => e.UserRoleId);

                entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Roles");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users");

                entity.HasAlternateKey(c => new { c.UserId, c.RoleId }).HasName("IX_UserRoles_UserID_RoleID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.NameIdentifier)
                    .IsRequired()
                    .HasColumnName("NameIdentifier")
                    .HasMaxLength(100);

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.ToTable("Websites");

                entity.HasKey(e => e.WebsiteId);

                entity.Property(e => e.WebsiteId).HasColumnName("WebsiteID");

                entity.Property(e => e.EnvironmentId).HasColumnName("EnvironmentID");

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

                entity.HasOne(d => d.Environment)
                    .WithMany(p => p.Websites)
                    .HasForeignKey(d => d.EnvironmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceInstance_Environments");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Websites)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceInstance_Services");
            });

            modelBuilder.Entity<TermsVersion>(entity =>
            {
                entity.ToTable("TermsVersions");

                entity.HasKey(e => e.TermsVersionId);

                entity.Property(t => t.TermsVersionId)
                    .HasColumnName("TermsVersionID")
                    .ValueGeneratedNever();

                entity.Property(x => x.CreatedByUserId)
                    .HasColumnName("CreatedByUserID");

                entity.Property(x => x.VersionDate)
                    .IsRequired();

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(x => x.UserCreatedTermsVersions)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TermsVersion_CreatedByUser");
            });

            modelBuilder.Entity<UserAcceptedTermsVersion>(entity =>
            {
                entity.ToTable("UserAcceptedTermsVersion");

                entity.HasKey(x => x.UserAcceptedTermsVersionId);

                entity.Property(e => e.UserAcceptedTermsVersionId)
                    .HasColumnName("UserAcceptedTermsVersionID");

                entity.Property(e => e.TermsVersionId)
                    .HasColumnName("TermsVersionID");

                entity.Property(x => x.UserAcceptedDate)
                    .IsRequired();

                entity.HasOne(x => x.TermsVersion)
                    .WithMany(x => x.UserAcceptedTermsVersions)
                    .HasForeignKey(x => x.TermsVersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAcceptedTermsVersion_TermsVersion");

                entity.HasOne(x => x.User)
                    .WithMany(x => x.UserAcceptedTermsVersions)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAcceptedTermsVersion_User");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Jobs");

                entity.HasKey(e => e.JobId);

                entity.Property(e => e.JobId).HasColumnName("JobID");
                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.OrganisationId).HasColumnName("OrganisationID");

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Jobs)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Jobs_User");

                entity.HasOne(x => x.Organisation)
                    .WithMany(x => x.Jobs)
                    .HasForeignKey(x => x.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Jobs_Organisation");
            });

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.ToTable("Organisations");

                entity.HasKey(e => e.OrganisationId);

                entity.Property(e => e.OrganisationId).HasColumnName("OrganisationID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<OrganisationRole>(entity =>
            {
                entity.ToTable("OrganisationRoles");

                entity.HasKey(e => e.OrganisationRoleId);

                entity.Property(e => e.OrganisationRoleId).HasColumnName("OrganisationRoleID");
                entity.Property(e => e.OrganisationId).HasColumnName("OrganisationID");
                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(x => x.Organisation)
                    .WithMany(x => x.OrganisationRoles)
                    .HasForeignKey(x => x.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationRole_Organisation");

                entity.HasOne(x => x.Role)
                    .WithMany(x => x.OrganisationRoles)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationRole_Role");
            });
        }
    }
}
