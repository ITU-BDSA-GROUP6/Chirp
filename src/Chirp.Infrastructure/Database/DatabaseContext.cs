using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Chirp.Models;

/*
    @DESCRIPTION:
        - In following is a subclass of the class 'DbContext'.
        - It follows the behavioural pattern 'Unit of Work'. Simply put, this is everything one does in a single business transaction 
          which may alter a databas - [https://en.wikipedia.org/wiki/Unit_of_work].

    @KEY INFO:
        - Our 'Models' folder contains the "Entities" of our SQLite database.
            -> In DBMS, an entity is a piece of data tracked and stored by the system. 
        - The ModelBuilder class acts as a Fluent API, a design pattern based on method chaining - [https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx].
        
*/

namespace DBContext;
public class DatabaseContext : IdentityDbContext<Author>
{
    public virtual DbSet<Cheep> Cheeps { get; set; }
    public virtual DbSet<Author> Authors { get; set; }

    public DatabaseContext()
    {
        // Must have empty constructor for Compile-Time Migrtion to work.
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>(entity => 
        {
            entity.ToTable("Authors");
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.Email).IsUnique();
            //entity.Property(a => a.UserName).HasMaxLength(20); (Needs to match with Registration Limit)
            // ... more config ... 
        });

        modelBuilder.Entity<Cheep>(entity => 
        {
            entity.ToTable("Cheeps");
            entity.Property(cheep => cheep.Text).HasMaxLength(160);
        });



        modelBuilder.Entity<Cheep>().ToTable("Cheeps");
        modelBuilder.Entity<Author>().ToTable("Authors");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string databasePath = Path.Combine(Path.GetTempPath(), "chirp.db");
        Console.WriteLine("Database Path: " + databasePath);

        if(!File.Exists(databasePath)) Console.WriteLine("The database file does NOT exist");

        optionsBuilder.UseSqlite($"Data Source={databasePath}");
    }
}