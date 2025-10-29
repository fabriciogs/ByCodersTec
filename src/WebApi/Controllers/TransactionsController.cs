using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Provides endpoints for managing and retrieving transaction data.
    /// </summary>
    /// <remarks>The <c>TransactionsController</c> class handles HTTP requests related to transaction
    /// processing, including uploading CNAB files, retrieving transactions by store, fetching store balances, and
    /// listing all store names. It utilizes asynchronous operations to interact with the transaction service and logs
    /// critical errors during processing.</remarks>
    /// <param name="logger"></param>
    /// <param name="transactionService"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController(ILogger<TransactionsController> logger, ITransactionService transactionService) : ControllerBase
    {
        /// <summary>
        /// Handles the upload of CNAB file data for processing.
        /// </summary>
        /// <remarks>This method processes the uploaded CNAB file data asynchronously. It logs critical
        /// errors if exceptions occur during processing.</remarks>
        /// <param name="dataLines">A list of strings representing the lines of data from the CNAB file. Must not be null or empty.</param>
        /// <param name="fileName">The name of the file being uploaded. Must not be null or empty.</param>
        /// <param name="userId">An optional identifier for the user performing the upload.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the upload operation. Returns <see
        /// cref="BadRequestResult"/> if the input is invalid or an error occurs, otherwise returns <see
        /// cref="OkResult"/> upon successful processing.</returns>
        [HttpPost("import")]
        public async Task<IActionResult> ImportLinesAsync(List<string> dataLines, string fileName, string? userId)
        {
            if (dataLines == null || dataLines.Count == 0)
                return BadRequest("Invalid data");

            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid file name");

            try
            {
                await transactionService.ProcessCnabFileAsync(dataLines, fileName, userId);
                return Ok("File processed successfully");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all transactions associated with the specified store.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch transactions from the data
        /// source.</remarks>
        /// <param name="storeName">The name of the store for which to retrieve transactions. Cannot be null or empty.</param>
        /// <returns>An <see cref="IActionResult"/> containing a collection of transactions for the specified store.</returns>
        [HttpGet("store/{storeName}")]
        public async Task<IActionResult> GetByStore(string storeName)
        {
            var transactions = await transactionService.GetTransactionsByStoreAsync(storeName);
            return Ok(transactions);
        }

        /// <summary>
        /// Retrieves the current balance for the specified store.
        /// </summary>
        /// <remarks>This method calls the transaction service to obtain the balance for the specified
        /// store and returns it in the response.</remarks>
        /// <param name="storeName">The name of the store for which to retrieve the balance. Cannot be null or empty.</param>
        /// <returns>An <see cref="IActionResult"/> containing the store name and its current balance.</returns>
        [HttpGet("store/{storeName}/balance")]
        public async Task<IActionResult> GetBalance(string storeName)
        {
            var balance = await transactionService.GetBalanceByStoreAsync(storeName);
            return Ok(new { StoreName = storeName, Balance = balance });
        }

        /// <summary>
        /// Retrieves a list of all store names.
        /// </summary>
        /// <remarks>This method sends an HTTP GET request to fetch the names of all available stores. The
        /// result is returned as an HTTP 200 OK response containing the list of store names.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a list of store names. The response is an HTTP 200 OK with the
        /// list of store names if successful.</returns>
        [HttpGet("stores")]
        public async Task<IActionResult> GetAllStores()
        {
            var stores = await transactionService.GetAllStoreNamesAsync();
            return Ok(stores);
        }
    }
}