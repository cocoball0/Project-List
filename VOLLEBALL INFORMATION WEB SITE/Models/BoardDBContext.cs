using Microsoft.EntityFrameworkCore;

namespace Mini_Project2.Models
{
    public class BoardDbContext : DbContext
    {
        public BoardDbContext(DbContextOptions<BoardDbContext> options) : base(options)
        {
        }
        public DbSet<boards> Board { get; set; }
    }
}
