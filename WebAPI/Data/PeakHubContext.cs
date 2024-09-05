using WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Data
{
    public class PeakHubContext : DbContext
    {
        public PeakHubContext(DbContextOptions<PeakHubContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
