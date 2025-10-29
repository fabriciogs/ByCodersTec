using Domain.Entities;

namespace Application.Dtos
{
    public class StoreSummary
    {
        public string Name { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; } = [];
    }
}