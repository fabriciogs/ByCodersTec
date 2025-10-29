namespace Infrastructure.DapperDataAccess.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid ImportFileId { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public decimal Value { get; set; }
        public string Cpf { get; set; }
        public string Card { get; set; }
        public string StoreOwner { get; set; }
        public string StoreName { get; set; }
    }
}