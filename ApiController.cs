using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {

	private readonly HttpClient _httpClient;
        private readonly string _pythonWorkerUrl = "http://ip172-18-0-8-cr9l76qim2rg00fl4620-5000.direct.labs.play-with-docker.com/procesar-html";
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

            if (response.IsSuccessStatusCode)
            {
                // Leer el contenido del JSON existente
                var responseContent = await response.Content.ReadAsStringAsync();
                var existingJsonResponse = JsonDocument.Parse(responseContent).RootElement;

                // Convertir el JSON existente a un objeto mutable
                var mutableResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                // Añadir el resultado de la conexión a la base de datos
                mutableResponse["DatabaseConnection"] = JsonDocument.Parse(JsonSerializer.Serialize(new { ConnectionStatus = canConnect ? "successful" : "failed" })).RootElement;

                // Crear el JSON actualizado
                var updatedJsonResponse = JsonSerializer.Serialize(mutableResponse);

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