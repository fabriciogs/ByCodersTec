using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(ITransactionService transactionService) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            try
            {
                using var stream = file.OpenReadStream();
                await transactionService.ProcessCnabFileAsync(stream);
                return Ok("File processed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("store/{storeName}")]
        public async Task<IActionResult> GetByStore(string storeName)
        {
            var transactions = await transactionService.GetTransactionsByStoreAsync(storeName);
            return Ok(transactions);
        }

        [HttpGet("store/{storeName}/balance")]
        public async Task<IActionResult> GetBalance(string storeName)
        {
            var balance = await transactionService.GetBalanceByStoreAsync(storeName);
            return Ok(new { StoreName = storeName, Balance = balance });
        }

        [HttpGet("stores")]
        public async Task<IActionResult> GetAllStores()
        {
            var stores = await transactionService.GetAllStoreNamesAsync();
            return Ok(stores);
        }
    }
}