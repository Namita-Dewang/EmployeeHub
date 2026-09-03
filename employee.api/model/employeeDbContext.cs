using Microsoft.EntityFrameworkCore;

namespace employee.api.model
{
    public class employeeDbContext : DbContext
    {
        public employeeDbContext(DbContextOptions<employeeDbContext> options) : base(options)
        {

        }

        public DbSet<employeemodel> employees { get; set; }
        public DbSet<department> departments { get; set; }
        public DbSet<designation> designations { get; set; }
    }
}
