namespace Infrastructure.DapperDataAccess.Entities
{
    public class ImportFile
    {
        public Guid Id { get; set; }
        public DateTime ImportDate { get; set; }
        public string FileName { get; set; }
        public string? UserId { get; set; }
        public int TotalRows { get; set; }
        public int ImportedRows { get; set; }
    }
}