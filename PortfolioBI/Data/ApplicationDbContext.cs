using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioBI.Models;

namespace PortfolioBI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<FinancialData> FinancialData { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
