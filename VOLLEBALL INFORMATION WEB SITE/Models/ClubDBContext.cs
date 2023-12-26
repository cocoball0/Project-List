using Microsoft.EntityFrameworkCore;

namespace Mini_Project2.Models
{
	public class ClubDBContext : DbContext
	{
		public ClubDBContext(DbContextOptions<ClubDBContext> options) : base(options)
		{
		}
		public DbSet<Club> Club { get; set; }
	}
}
