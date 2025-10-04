using AuthMicroservice.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.DAL
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<
    UserDal,
    RoleDal,
    Guid,
    IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>
    >(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserDal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            builder.Entity<RoleDal>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Creator)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            SeedBaseRoles(builder);
        }

        private void SeedBaseRoles(ModelBuilder builder)
        {
            var adminRoleId = Guid.Parse("A1B2C3D4-E5F6-4789-0123-456789ABCDEF");
            var instructorRoleId = Guid.Parse("B2C3D4E5-F6A7-5890-1234-56789ABCDEF0");
            var studentRoleId = Guid.Parse("C3D4E5F6-A7A8-6901-2345-6789ABCDEF01");

            // Static created timestamp to avoid model changes between builds
            var staticCreatedAt = new DateTime(1984, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Deterministic concurrency stamps (avoid Guid.NewGuid())
            const string adminConcurrencyStamp = "11111111-1111-1111-1111-111111111111";
            const string instructorConcurrencyStamp = "22222222-2222-2222-2222-222222222222";
            const string studentConcurrencyStamp = "33333333-3333-3333-3333-333333333333";

            builder.Entity<RoleDal>().HasData(
                new RoleDal
                {
                    Id = adminRoleId,
                    Name = "Administrator",
                    CreatedAt = staticCreatedAt,
                    ConcurrencyStamp = adminConcurrencyStamp
                },
                new RoleDal
                {
                    Id = instructorRoleId,
                    Name = "Instructor",
                    CreatedAt = staticCreatedAt,
                    ConcurrencyStamp = instructorConcurrencyStamp
                },
                new RoleDal
                {
                    Id = studentRoleId,
                    Name = "Student",
                    CreatedAt = staticCreatedAt,
                    ConcurrencyStamp = studentConcurrencyStamp
                }
            );
        }
    }
}