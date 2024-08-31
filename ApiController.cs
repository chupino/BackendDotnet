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
        private readonly string _pythonWorkerUrl = "http://ip172-18-0-24-cr970ciim2rg00fp9ulg-5000.direct.labs.play-with-docker.com/procesar-html";
	
        public ApiController(HttpClient httpClient)
        {
	    _httpClient = httpClient;
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
		 var responseContent = await response.Content.ReadAsStringAsync();
                 return Content(responseContent, "text/plain"); // Devolver el contenido como texto plano
            }
            return StatusCode((int)response.StatusCode, "Error al enviar el contenido al worker de Python");
            
        }
    }
}