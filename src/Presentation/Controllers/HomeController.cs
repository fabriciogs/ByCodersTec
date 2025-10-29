using Microsoft.AspNetCore.Mvc;
using Presentation.WebApi;

namespace Presentation.Controllers
{
    public class HomeController() : Controller
    {
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a valid file.";
                return View("Index");
            }

            // Read file lines
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var dataLines = new List<string>();
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                dataLines.Add(line);
            }

            try
            {
                var webApiClient = new WebApiClient();
                await webApiClient.ImportLinesAsync(dataLines, file.FileName, "fabricio");
                ViewBag.Success = "File processed successfully! Check the Transactions page for more details and account balance.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error processing file: {ex}";
                throw;
            }

            return View("Index");
        }
    }
}