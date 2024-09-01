using Microsoft.EntityFrameworkCore;
public static class DataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var baseStorageUrl="http://ip172-18-0-41-cr9r9e2im2rg00fl4om0-6000.direct.labs.play-with-docker.com/";
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