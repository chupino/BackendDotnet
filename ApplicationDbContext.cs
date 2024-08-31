using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Define las entidades (tablas) aquí
    public DbSet<HTMLFile> HtmlFiles { get; set; }
}

public class HTMLFile
{
    public int Id { get; set; }
    public string Path { get; set; }
}