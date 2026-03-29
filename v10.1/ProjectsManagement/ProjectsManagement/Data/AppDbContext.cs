using Microsoft.EntityFrameworkCore;
using ProjectsManagement.Models;

namespace ProjectsManagement.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) :DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
    }
}
