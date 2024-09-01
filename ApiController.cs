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
        private readonly string _pythonWorkerUrl = "http://ip172-18-0-13-cr9r9e2im2rg00fl4om0-5000.direct.labs.play-with-docker.com/procesar-html";
        private readonly ApplicationDbContext _context;
	
        public ApiController(HttpClient httpClient,ApplicationDbContext context)
        {
	        _httpClient = httpClient;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetHtmlContent([FromQuery] string query = "")
        {
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
            }

            var jsonContent = new
            {
                dataset = dataset,
                query = query
            };

            var jsonString = JsonSerializer.Serialize(jsonContent);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Enviar el JSON al worker de Python
            var response = await _httpClient.PostAsync(_pythonWorkerUrl, content);

            // Verificar la conexión a la base de datos
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

            if (response.IsSuccessStatusCode)
            {
                // Leer el contenido del JSON existente
                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Convertir el JSON existente a un diccionario
                var existingJsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

                // Añadir el resultado de la conexión a la base de datos
                existingJsonResponse["DatabaseConnection"] = new { ConnectionStatus = canConnect ? "successful" : "failed" };
                existingJsonResponse["HtmlFileRecords"] = htmlFileRecords;

                // Crear el JSON actualizado
                var updatedJsonResponse = JsonSerializer.Serialize(existingJsonResponse);

                // Devolver el JSON actualizado
                return new ContentResult
                {
                    Content = updatedJsonResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }

            return StatusCode((int)response.StatusCode, "Error al enviar el contenido al worker de Python");
        }
    }
}