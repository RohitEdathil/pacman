using Microsoft.EntityFrameworkCore;
using pacman.Models;

namespace pacman.Database;

public class PacmanDbContext : DbContext 
{
    public PacmanDbContext(DbContextOptions<PacmanDbContext> options) : base(options){}

    public DbSet<Transaction>? Transactions { get; set; }
    public DbSet<Pending>? Pendings { get; set; }
}