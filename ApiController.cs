using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class HtmlController : ControllerBase
    {
        private readonly string _htmlFilePath;

        public HtmlController()
        {
            // Ruta del archivo HTML en la carpeta del proyecto
            _htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "htmls", "doc1.html");
        }

        [HttpGet]
        public async Task<IActionResult> GetHtmlContent()
        {
            if (System.IO.File.Exists(_htmlFilePath))
            {
                var htmlContent = await System.IO.File.ReadAllTextAsync(_htmlFilePath);
                return Content(htmlContent, "text/plain"); // Devolver el contenido como texto plano
            }
            return NotFound();
        }
    }
}