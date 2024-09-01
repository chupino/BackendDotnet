using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Backend;

namespace Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {

	private readonly HttpClient _httpClient;
        private readonly string _pythonWorkerUrl = "http://ip172-18-0-46-crabgcqim2rg00dvbah0-5000.direct.labs.play-with-docker.com/procesar-html";
        private readonly ApplicationDbContext _context;
	
        public ApiController(HttpClient httpClient,ApplicationDbContext context)
        {
	        _httpClient = httpClient;
	        _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetHtmlContent([FromQuery] string query = "")
        {
            /*
            var htmlDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "htmls");
            if (!Directory.Exists(htmlDirectoryPath))
            {
                return NotFound("El directorio 'htmls' no existe.");
            }

            var htmlFiles = Directory.GetFiles(htmlDirectoryPath, "*.html");
            var dataset = new List<object>();
            int idCounter = 1;
            foreach (var filePath in htmlFiles)
            {
                var htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
                dataset.Add(new
                {
                    id = idCounter++,
                    path = filePath,
                    content = htmlContent
                });
            }*/

            bool canConnect;
            IEnumerable<HTMLFile> htmlFileRecords = null;
            try
            {
                canConnect =  _context.Database.CanConnect();
                htmlFileRecords = await _context.HtmlFiles.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Database connection failed: {ex.Message}" });
            }

            var htmlFilesWithContent = new List<object>();

            foreach (var htmlFile in htmlFileRecords)
            {
                try
                {
                    var htmlContent = await _httpClient.GetStringAsync(htmlFile.Path);
                    htmlFilesWithContent.Add(new {
                        id = htmlFile.Id,
                        path = htmlFile.Path,
                        content = htmlContent
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error fetching HTML content from {htmlFile.Path}: {ex.Message}");
                }
            }


            var jsonContent = new
            {
                dataset = htmlFilesWithContent,
                query = query
            };

            var jsonString = JsonSerializer.Serialize(jsonContent);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_pythonWorkerUrl, content);            

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var serializeJsonResponse = JsonSerializer.Serialize(JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent));

                return new ContentResult
                {
                    Content = serializeJsonResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }

            return StatusCode((int)response.StatusCode, "Error al enviar el contenido al worker de Python");
        }
    }
}
