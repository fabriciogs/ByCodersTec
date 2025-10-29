using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class TransactionsController(ITransactionService transactionService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string storeName)
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