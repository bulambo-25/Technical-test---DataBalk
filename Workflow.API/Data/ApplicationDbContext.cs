using Microsoft.EntityFrameworkCore;
using Workflow.API.Models;

namespace Workflow.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        
        }
        public DbSet<User> tUser { get; set; }
        public DbSet<Tasks> tTask { get; set; }
    }
}
