using Microsoft.EntityFrameworkCore;
using Data.Database.Entities;
using Data.Database.Entities.User;
using Data.Database.Entities.Authentication;

namespace Data.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<UserProfileEntity> UserProfiles => Set<UserProfileEntity>();
        public DbSet<UserAddressEntity> UserAddresses => Set<UserAddressEntity>();
        public DbSet<RoleEntity> Roles => Set<RoleEntity>();
        public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
        public DbSet<ModulePermissionEntity> ModulePermissions => Set<ModulePermissionEntity>();
        public DbSet<ModuleEntity> Modules  => Set<ModuleEntity>();
        public DbSet<UserCredentialsEntity> UserCredentials => Set<UserCredentialsEntity>();
        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(builder =>
            {
                builder.Property(x => x.UserName).HasMaxLength(128).IsRequired();
                builder.Property(x => x.Email).HasMaxLength(320).IsRequired();

                builder.HasIndex(x => x.UserName).IsUnique();
                builder.HasIndex(x => x.Email).IsUnique();

                builder.HasOne(x => x.Credentials)
                    .WithOne(x => x.User)
                    .HasForeignKey<UserCredentialsEntity>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(x => x.Profile)
                    .WithOne(x => x.User)
                    .HasForeignKey<UserProfileEntity>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserProfileEntity>(builder =>
            {
                builder.Property(x => x.FirstName).HasMaxLength(128);
                builder.Property(x => x.LastName).HasMaxLength(128);
                builder.HasIndex(x => x.UserId).IsUnique();

                builder.HasOne(x => x.Address)
                    .WithOne(x => x.UserProfile)
                    .HasForeignKey<UserAddressEntity>(x => x.UserProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserAddressEntity>(builder =>
            {
                builder.Property(x => x.Street).HasMaxLength(256).IsRequired();
                builder.Property(x => x.HouseNumber).HasMaxLength(32).IsRequired();
                builder.Property(x => x.PostalCode).HasMaxLength(32).IsRequired();
                builder.Property(x => x.City).HasMaxLength(128).IsRequired();
                builder.Property(x => x.StateOrProvince).HasMaxLength(128);
                builder.Property(x => x.CountryCode).HasMaxLength(2).IsRequired();

                builder.HasIndex(x => x.UserProfileId).IsUnique();
                builder.HasIndex(x => new { x.CountryCode, x.PostalCode, x.City });
            });

            modelBuilder.Entity<UserCredentialsEntity>(builder =>
            {
                builder.Property(x => x.PasswordHash).HasMaxLength(1024).IsRequired();
                builder.Property(x => x.FaildLoginAttemts).IsRequired();
                builder.HasIndex(x => x.UserId).IsUnique();
            });

            modelBuilder.Entity<RoleEntity>(builder =>
            {
                builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
                builder.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<UserRoleEntity>(builder =>
            {
                builder.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();

                builder.HasOne(x => x.User)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(x => x.Role)
                    .WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ModulePermissionEntity>(entity =>
            {
                entity.HasOne(x => x.User)
                    .WithMany(x => x.ModulePermissions)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Module)
                    .WithMany(x => x.ModulePermissions)
                    .HasForeignKey(x => x.ModuleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.UserId, x.ModuleId })
                    .IsUnique();
            });

            modelBuilder.Entity<RefreshTokenEntity>(builder =>
            {
                builder.Property(x => x.TokenHash).HasMaxLength(256).IsRequired();
                builder.Property(x => x.CreatedByIp).HasMaxLength(64).IsRequired();
                builder.Property(x => x.RevokedByIp).HasMaxLength(64);
                builder.Property(x => x.ReplacedByTokenHash).HasMaxLength(256);
                builder.HasIndex(x => x.TokenHash).IsUnique();

                builder.HasOne(x => x.UserCredentials)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.UserCredentialsId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
