using Microsoft.EntityFrameworkCore;

namespace KCKwebowka.Models
{
    public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=KCKwebowka;Trusted_Connection=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
