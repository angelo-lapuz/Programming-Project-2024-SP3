using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new PeakHubContext(
                serviceProvider.GetRequiredService<DbContextOptions<PeakHubContext>>());

            // Look for users in DB, change this to Tasks later on.
            if (context.Users.Any())
                return; // DB has been seeded.

         
            context.SaveChanges();
        }
    }
}
