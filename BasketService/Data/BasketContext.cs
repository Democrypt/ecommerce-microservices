using Microsoft.EntityFrameworkCore;

namespace BasketService.Data
{
    public class BasketContext : DbContext
    {
        public BasketContext(DbContextOptions<BasketContext> options) : base(options) { }

        public DbSet<BasketItem> BasketItems { get; set; }
    }
}
