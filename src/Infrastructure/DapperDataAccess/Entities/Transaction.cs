namespace Infrastructure.DapperDataAccess.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Value { get; set; }
        public string Cpf { get; set; }
        public string Card { get; set; }
        public string StoreOwner { get; set; }
        public string StoreName { get; set; }
        public DateTime ImportDate { get; set; }
        public Guid FileImportId { get; set; }
    }
}