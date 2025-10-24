using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class HomeController(ITransactionService transactionService) : Controller
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

            try
            {
                using var stream = file.OpenReadStream();
                await transactionService.ProcessCnabFileAsync(stream);
                ViewBag.Success = "File processed successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error processing file: {ex.Message}";
            }

            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Transactions(string storeName)
        {
            var stores = await transactionService.GetAllStoreNamesAsync();
            ViewBag.Stores = stores;

            if (string.IsNullOrWhiteSpace(storeName))
                return View(new List<Domain.Entities.Transaction>());

            var transactions = await transactionService.GetTransactionsByStoreAsync(storeName);
            var balance = await transactionService.GetBalanceByStoreAsync(storeName);
            ViewBag.Balance = balance;
            ViewBag.SelectedStore = storeName;

            return View(transactions);
        }
    }
}