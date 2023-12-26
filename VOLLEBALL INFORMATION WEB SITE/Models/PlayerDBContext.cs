using Microsoft.EntityFrameworkCore;

namespace Mini_Project2.Models
{
	public class PlayerDBcontext : DbContext
	{
		public PlayerDBcontext(DbContextOptions<PlayerDBcontext> options) : base(options)
		{
		}
		public DbSet<Player> Player { get; set; }
	}
}
