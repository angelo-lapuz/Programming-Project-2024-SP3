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


            builder.Entity<User>()
                .HasMany(u => u.Awards)
                .WithMany(a => a.Users)
                .UsingEntity(j => j.ToTable("UserAwards"));

            builder.Entity<User>()
                .HasMany(u => u.Peaks)
                .WithMany(p => p.Users)
                .UsingEntity(j => j.ToTable("UserPeaks"));

            builder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserID);

            builder.Entity<Like>()
                .HasKey(l => new { l.UserID, l.PostID });

            builder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserID);

            builder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostID);
        }
    }
}