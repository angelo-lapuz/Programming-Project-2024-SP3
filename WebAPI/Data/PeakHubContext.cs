using WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Data
{
    public class PeakHubContext : IdentityDbContext<User>
    {
        public PeakHubContext(DbContextOptions<PeakHubContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Peak> Peaks { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Award> Awards { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define Roles
            const string ADMIN_ID = "admin-role-id";
            const string USER_ID = "user-role-id";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = ADMIN_ID, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = USER_ID, Name = "User", NormalizedName = "USER" }
            );

            builder.Entity<UserAward>()
                .HasKey(ua => ua.Id);

            builder.Entity<UserAward>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAwards)
                .HasForeignKey(ua => ua.UserID);

            builder.Entity<UserAward>()
                .HasOne(ua => ua.Award)
                .WithMany(a => a.UserAwards)
                .HasForeignKey(ua => ua.AwardID);

            builder.Entity<UserAward>()
                .ToTable("UserAwards");

            builder.Entity<UserPeak>()
                .HasKey(up => up.Id);

            builder.Entity<UserPeak>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPeaks)
                .HasForeignKey(up => up.UserID);

            builder.Entity<UserPeak>()
                .HasOne(up => up.Peak)
                .WithMany(p => p.UserPeaks)
                .HasForeignKey(up => up.PeakID);

            builder.Entity<UserPeak>()
                .ToTable("UserPeaks");

            builder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            builder.Entity<Board>()
                .HasMany(b => b.Posts)
                .WithOne(p => p.Board)
                .HasForeignKey(p => p.BoardID);

            builder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId);

            builder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostID);

            builder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.PostID });
        }




    }
}
