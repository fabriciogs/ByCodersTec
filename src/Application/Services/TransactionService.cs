using Application.Dtos;
using Application.Repositories;
using Domain.Entities;
using FixedWidthParserWriter;
using FluentValidation;

namespace Application.Services
{
    public class TransactionService(ITransactionRepository repository, IValidator<TransactionDto> validator) : ITransactionService
    {
        public async Task ProcessCnabFileAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            var dataLines = new List<string>();
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                dataLines.Add(line);
            }

            var transactions = new List<Transaction>();
            var transactionLines = new FixedWidthLinesProvider<TransactionDto>().Parse(dataLines);
            foreach (var dto in transactionLines)
            {
                await validator.ValidateAndThrowAsync(dto);
                var transaction = dto.ToEntity();
                transactions.Add(transaction);
            }

            var fileImportId = Guid.NewGuid();
            await repository.AddRangeAsync(fileImportId, transactions);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStoreAsync(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
                throw new ArgumentException("Store name is required");

            return await repository.GetByStoreNameAsync(storeName);
        }

        public async Task<decimal> GetBalanceByStoreAsync(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
                throw new ArgumentException("Store name is required");

            return await repository.GetBalanceByStoreNameAsync(storeName);
        }

        public async Task<IEnumerable<string>> GetAllStoreNamesAsync() => await repository.GetAllStoreNamesAsync();
    }

    internal static class TransactionMapper
    {
        public static Transaction ToEntity(this TransactionDto dto)
            => new(
                  dto.Id
                , dto.Type
                , dto.DateTime.ToString("yyyyMMdd")
                , dto.Value
                , dto.Cpf
                , dto.Card
                , dto.DateTime.ToString("HHmmss")
                , dto.StoreOwner
                , dto.StoreName
                    );
    }
}