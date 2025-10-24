using Application.Repositories;
using Dapper;
using Infrastructure.DapperDataAccess.Entities;
using Infrastructure.Mappings;

namespace Infrastructure.Repositories
{
    public class TransactionRepository(IDbConnectionFactory connectionFactory) : ITransactionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task AddRangeAsync(Guid fileImportId, IEnumerable<Domain.Entities.Transaction> transactions)
        {
            // Map Domain Entities to DB Entities
            var dbe = new List<Transaction>();
            foreach (var item in transactions) dbe.Add(item.ToDbEntity(fileImportId));

            using var conn = await _connectionFactory.CreateConnectionAsync();
            await conn.ExecuteAsync(
                @"INSERT INTO Transactions (Id, Type, DateTime, Value, Cpf, Card, StoreOwner, StoreName, ImportDate, FileImportId)
                  VALUES (@Id, @Type, @DateTime, @Value, @Cpf, @Card, @StoreOwner, @StoreName, @ImportDate, @FileImportId)",
                dbe);
        }

        public async Task<IEnumerable<Domain.Entities.Transaction>> GetByStoreNameAsync(string storeName)
        {
            using var conn = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT Id, Type, DateTime, Value, Cpf, Card, StoreOwner, StoreName, ImportDate, FileImportId FROM Transactions WITH (NOLOCK) WHERE StoreName = @StoreName";
            var dbe = await conn.QueryAsync<Transaction>(sql, new { StoreName = storeName });

            // Map DB Entities to Domain Entities
            var results = dbe.ToList();
            var entities = new List<Domain.Entities.Transaction>();
            foreach (var item in results) entities.Add(item.ToEntity());

            return entities;
        }

        public async Task<decimal> GetBalanceByStoreNameAsync(string storeName)
        {
            using var conn = await _connectionFactory.CreateConnectionAsync();
            var transactions = await GetByStoreNameAsync(storeName);
            return transactions.Sum(t => t.GetSignedValue());
        }

        public async Task<IEnumerable<string>> GetAllStoreNamesAsync()
        {
            using var conn = await _connectionFactory.CreateConnectionAsync();
            return await conn.QueryAsync<string>("SELECT DISTINCT StoreName FROM Transactions WITH (NOLOCK)");
        }
    }
}