public static class DataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Insertar datos iniciales en la tabla 'Example'
        modelBuilder.Entity<HTMLFile>().HasData(
            new HTMLFile { Id = 1, Path = "Sample Data 1" },
            new HTMLFile { Id = 2, Path = "Sample Data 2" }
        );
    }
}