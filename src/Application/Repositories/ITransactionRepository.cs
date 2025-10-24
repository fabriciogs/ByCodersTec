using Domain.Entities;

namespace Application.Repositories
{
    public interface ITransactionRepository
    {
        Task AddRangeAsync(Guid fileImportId, IEnumerable<Transaction> transactions);
        Task<IEnumerable<Transaction>> GetByStoreNameAsync(string storeName);
        Task<decimal> GetBalanceByStoreNameAsync(string storeName);
        Task<IEnumerable<string>> GetAllStoreNamesAsync();
    }
}