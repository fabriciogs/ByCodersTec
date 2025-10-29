using Application.Repositories;
using Dapper;
using Infrastructure.DapperDataAccess.Entities;
using Infrastructure.Mappings;
using DE = Domain.Entities;

namespace Infrastructure.Repositories
{
    public class TransactionRepository(IDbConnectionFactory connectionFactory) : ITransactionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

        public async Task SaveTransactionsAsync(DE.ImportFile importFile, IEnumerable<DE.Transaction> transactions)
        {
            // Map Domain Entities to DB Entities
            var dbeImportFile = importFile.ToDbEntity();
            var dbeTransactions = new List<Transaction>();
            foreach (var item in transactions) dbeTransactions.Add(item.ToDbEntity());

            // Open connection and begin transaction
            using var conn = await _connectionFactory.CreateConnectionAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                // Insert ImportFile record
                await conn.ExecuteAsync(
                @"INSERT INTO ImportFiles (Id, ImportDate, [FileName], UserId, TotalRows, ImportedRows)
                  VALUES (@Id, @ImportDate, @FileName, @UserId, @TotalRows, @ImportedRows)",
                dbeImportFile, transaction: tran);

                // Insert Transaction records
                await conn.ExecuteAsync(
                @"INSERT INTO Transactions (Id, ImportFileId, TransactionTypeId, OccurrenceDate, [Value], Cpf, Card, StoreOwner, StoreName)
                  VALUES (@Id, @ImportFileId, @TransactionTypeId, @OccurrenceDate, @Value, @Cpf, @Card, @StoreOwner, @StoreName)",
                dbeTransactions, transaction: tran);

                // Commit transaction
                tran.Commit();
            }
            catch (Exception)
            {
                // Rollback transaction on error
                tran.Rollback();

                // Rethrow exception
                throw;
            }
        }

        public async Task<IEnumerable<DE.Transaction>> GetByStoreNameAsync(string storeName)
        {
            using var conn = await _connectionFactory.CreateConnectionAsync();
            var sql = "SELECT Id, ImportFileId, TransactionTypeId, OccurrenceDate, Value, Cpf, Card, StoreOwner, StoreName FROM Transactions WITH (NOLOCK) WHERE StoreName = @StoreName";
            var dbe = await conn.QueryAsync<Transaction>(sql, new { StoreName = storeName });

            // Map DB Entities to Domain Entities
            var results = dbe.ToList();
            var entities = new List<DE.Transaction>();
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

        public async Task<IEnumerable<DE.Transaction>> GetAllAsync()
        {
            using var conn = await _connectionFactory.CreateConnectionAsync();
            var dbe = await conn.QueryAsync<Transaction>("SELECT * FROM Transactions WITH (NOLOCK)");

            // Map DB Entities to Domain Entities
            var results = dbe.ToList();
            var entities = new List<DE.Transaction>();
            foreach (var item in results) entities.Add(item.ToEntity());

            return entities;
        }
    }
}