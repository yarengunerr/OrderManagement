using Microsoft.EntityFrameworkCore;
using SiparisYonetim.Models;
using SiparisYonetim.Pages;

namespace SiparisYonetim.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<Order> Orders { get; set; }

    }
}
