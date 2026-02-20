using Microsoft.EntityFrameworkCore;
using UDPServer.Models;

namespace UDPServer.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
    
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=persons.db");
        }
    }
}