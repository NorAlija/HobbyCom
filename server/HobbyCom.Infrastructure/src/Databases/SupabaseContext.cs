using HobbyCom.Domain.src.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HobbyCom.Infrastructure.src.Databases
{
    public class SupabaseContext : DbContext
    {
        public SupabaseContext(DbContextOptions<SupabaseContext> options)
                    : base(options)
        {
        }

        public DbSet<UserProfile> User { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<Refresh_Token> Refresh_Token { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("user_profiles", "public");

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();

                // entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Firstname).HasColumnName("first_name");
                entity.Property(e => e.Lastname).HasColumnName("last_name");
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.Username).HasColumnName("username").IsRequired();
                entity.Property(e => e.Phone).HasColumnName("phone").IsRequired(false);
                entity.Property(e => e.Role).HasColumnName("role")
                    .HasDefaultValue("USER")
                    .IsRequired(false)
                    .HasConversion(new ValueConverter<string?, string?>(
                        v => v == null ? null : v.ToUpperInvariant(),
                        v => v));
                entity.Property(e => e.Password).HasColumnName("encrypted_password").IsRequired();
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
                entity.Property(e => e.Created_At).HasColumnName("created_at")
                    .IsRequired(false)
                    .HasDefaultValueSql("current_timestamp");
                entity.Property(e => e.Updated_At).HasColumnName("updated_at")
                    .IsRequired(false)
                    .HasDefaultValueSql("current_timestamp");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("sessions", "public");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.User_Id).HasColumnName("user_id");
                entity.Property(e => e.Created_At).HasColumnName("created_at")
                    .IsRequired(false)
                    .HasDefaultValueSql("current_timestamp");
                entity.Property(e => e.Updated_At).HasColumnName("updated_at")
                    .IsRequired(false)
                    .HasDefaultValueSql("current_timestamp");
                entity.Property(e => e.Refreshed_At).HasColumnName("refreshed_at")
                    .IsRequired(false)
                    .HasDefaultValueSql("current_timestamp");
            });

            modelBuilder.Entity<Refresh_Token>(entity =>
            {
                entity.ToTable("refresh_tokens", "public");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.User_Id).HasColumnName("user_id");
                entity.Property(e => e.Token).HasColumnName("token");
                entity.Property(e => e.Created_At).HasColumnName("created_at")
                    .IsRequired(false)
                    .HasDefaultValueSql("current_timestamp");
                entity.Property(e => e.Token_Revoked).HasColumnName("token_revoked");
                entity.Property(e => e.Session_Id).HasColumnName("session_id");
            });
        }
    }
}