using Application.Dtos;
using Application.Repositories;
using Domain.Entities;
using FixedWidthParserWriter;
using System.Security.Cryptography.X509Certificates;

namespace Application.Services
{
    public class TransactionService(ITransactionRepository repository) : ITransactionService
    {
        public async Task ProcessCnabFileAsync(Stream fileStream, string fileName, string? userId)
        {
            // Read file lines
            using var reader = new StreamReader(fileStream);
            var dataLines = new List<string>();
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                dataLines.Add(line);
            }

            await ProcessCnabFileAsync(dataLines, fileName, userId);
        }

        public async Task ProcessCnabFileAsync(List<string> dataLines, string fileName, string? userId)
        {
            var totalLines = dataLines.Count;

            // Validate input lines (eliminate empty or too long lines)
            dataLines = [.. dataLines.Where(line => !string.IsNullOrWhiteSpace(line) && line.Length == 81)];
            if (dataLines.Count == 0) throw new ArgumentException("No valid data provided");

            // Parse and validate lines into transactions
            var transactions = new List<Transaction>();
            var transactionLines = new FixedWidthLinesProvider<TransactionDto>().Parse(dataLines);
            var importFile = new ImportFile(Guid.NewGuid(), fileName, userId);
            foreach (var dto in transactionLines)
            {
                var value = dto.Value / 100; //Transaction amount. Note: The value in the file must be divided by one hundred (value / 100.00) to normalize it.
                var transaction = new Transaction(importFile.Id, Guid.NewGuid(), dto.Type, dto.OccurrenceDate, value, dto.Cpf, dto.Card, dto.StoreOwner, dto.StoreName);
                transactions.Add(transaction);
            }

            // Update import file row counts
            importFile.UpdateRowCounts(totalLines, totalLines - (totalLines - transactions.Count));

            // Save transactions to repository
            await repository.SaveTransactionsAsync(importFile, transactions);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStoreAsync(string storeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(storeName, "Store name is required");
            return await repository.GetByStoreNameAsync(storeName);
        }

        public async Task<decimal> GetBalanceByStoreAsync(string storeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(storeName, "Store name is required");
            return await repository.GetBalanceByStoreNameAsync(storeName);
        }

        public async Task<IEnumerable<string>> GetAllStoreNamesAsync() => await repository.GetAllStoreNamesAsync();
    }
}