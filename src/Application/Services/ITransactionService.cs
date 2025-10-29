using Domain.Entities;

namespace Application.Services
{
    public interface ITransactionService
    {
        Task ProcessCnabFileAsync(List<string> dataLines, string fileName, string? userId);
        Task ProcessCnabFileAsync(Stream fileStream, string fileName, string? userId);
        Task<IEnumerable<Transaction>> GetTransactionsByStoreAsync(string storeName);
        Task<decimal> GetBalanceByStoreAsync(string storeName);
        Task<IEnumerable<string>> GetAllStoreNamesAsync();
    }
}