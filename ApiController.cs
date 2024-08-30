using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class HtmlController : ControllerBase
    {
        private readonly string _htmlFilePath;
	private readonly HttpClient _httpClient;
        private readonly string _pythonWorkerUrl = "http://ip172-18-0-60-cr947saim2rg00fp9dr0-5000.direct.labs.play-with-docker.com/procesar-html";
	
        public HtmlController(HttpClient httpClient)
        {
            // Ruta del archivo HTML en la carpeta del proyecto
            _htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "htmls", "doc1.html");
	    _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetHtmlContent()
        {
            if (System.IO.File.Exists(_htmlFilePath))
            {
                var htmlContent = await System.IO.File.ReadAllTextAsync(_htmlFilePath);

                var jsonContent = new
                {
                    html_list = new[] { htmlContent }
                };

                var jsonString = JsonSerializer.Serialize(jsonContent);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_pythonWorkerUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Content(htmlContent, "text/plain"); // Devolver el contenido como texto plano
                }
                return StatusCode((int)response.StatusCode, "Error al enviar el contenido al worker de Python");
            }
            return NotFound();
        }
    }
}