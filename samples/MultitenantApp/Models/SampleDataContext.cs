using Microsoft.EntityFrameworkCore;

namespace MultitenantApp.Models
{
    public class SampleDataContext: DbContext
    {
        public SampleDataContext(DbContextOptions<SampleDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Person> Persons { get; set; }
    }
}
