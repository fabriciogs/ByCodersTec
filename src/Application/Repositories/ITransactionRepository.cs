using Domain.Entities;

namespace Application.Repositories
{
    public interface ITransactionRepository
    {
        Task SaveTransactionsAsync(ImportFile importFile, IEnumerable<Transaction> transactions);
        Task<IEnumerable<Transaction>> GetByStoreNameAsync(string storeName);
        Task<decimal> GetBalanceByStoreNameAsync(string storeName);
        Task<IEnumerable<string>> GetAllStoreNamesAsync();
        Task<IEnumerable<Transaction>> GetAllAsync();
    }
}