using Microsoft.EntityFrameworkCore;
public static class DataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var baseStorageUrl="https://serverhtml.s3.us-east-1.amazonaws.com/";
        var htmlFiles = new List<HTMLFile>();
        for (int i = 1; i <= 8; i++)
        {
            var fileName = $"doc{i}.html";
            var fileUrl = $"{baseStorageUrl}{fileName}";

            htmlFiles.Add(new HTMLFile 
            { 
                Id = i, 
                Path = fileUrl
            });

        }
        modelBuilder.Entity<HTMLFile>().HasData(htmlFiles);
    }
}
