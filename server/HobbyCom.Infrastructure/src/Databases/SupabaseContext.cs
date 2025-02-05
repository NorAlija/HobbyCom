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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("userProfiles", "public");

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Firstname).HasColumnName("first_name");
                entity.Property(e => e.Lastname).HasColumnName("last_name");
                entity.Property(e => e.Email).HasColumnName("email").IsRequired();
                entity.Property(e => e.Username).HasColumnName("username").IsRequired();
                entity.Property(e => e.Phone).HasColumnName("phone").IsRequired(false);
                entity.Property(e => e.Type).HasColumnName("type")
                    .HasDefaultValue("USER")
                    .IsRequired(false)
                    .HasConversion(new ValueConverter<string?, string?>(
                        v => v == null ? null : v.ToUpperInvariant(),
                        v => v));
                entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at")
                    .IsRequired(false)
                    .HasDefaultValueSql("current_timestamp");
            });
        }
    }
}