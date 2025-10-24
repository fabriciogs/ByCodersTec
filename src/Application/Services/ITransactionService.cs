using Domain.Entities;

namespace Application.Services
{
    public interface ITransactionService
    {
        Task ProcessCnabFileAsync(Stream fileStream);
        Task<IEnumerable<Transaction>> GetTransactionsByStoreAsync(string storeName);
        Task<decimal> GetBalanceByStoreAsync(string storeName);
        Task<IEnumerable<string>> GetAllStoreNamesAsync();
    }
}