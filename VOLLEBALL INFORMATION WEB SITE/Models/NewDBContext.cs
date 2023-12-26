using Microsoft.EntityFrameworkCore;

namespace Mini_Project2.Models
{
    public class NewsDbContext : DbContext
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
        {
        }
        public DbSet<NewsItem> NewsItems { get; set; }
    }
}
