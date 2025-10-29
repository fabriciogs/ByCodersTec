namespace Domain.Entities
{
    public class ImportFile
    {
        public Guid Id { get; private set; }
        public DateTime ImportDate { get; private set; }
        public string FileName { get; private set; }
        public string? UserId { get; private set; }
        public int TotalRows { get; private set; }
        public int ImportedRows { get; private set; }

        public ImportFile(Guid id, string fileName, string? userId)
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName.Length <= 3) throw new ArgumentException("Invalid File Name");

            Id = id;
            ImportDate = DateTime.UtcNow;
            FileName = fileName;
            UserId = userId;
        }

        public ImportFile(Guid id, string fileName, string? userId, int totalRows, int importedRows)
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName.Length <= 4) throw new ArgumentException("Invalid File Name");
            if (totalRows <= 0) throw new ArgumentException("Total Rows must be positive");
            if (importedRows <= 0) throw new ArgumentException("Imported Rows must be positive");
            if (importedRows > totalRows) throw new ArgumentException("Imported Rows cannot be greater then the total of rows");

            Id = id;
            ImportDate = DateTime.UtcNow;
            FileName = fileName;
            UserId = userId;
            TotalRows = totalRows;
            ImportedRows = importedRows;
        }

        public void UpdateRowCounts(int totalRows, int importedRows)
        {
            if (totalRows <= 0) throw new ArgumentException("Total Rows must be positive");
            if (importedRows <= 0) throw new ArgumentException("Imported Rows must be positive");
            if (importedRows > totalRows) throw new ArgumentException("Imported Rows cannot be greater then the total of rows");

            TotalRows = totalRows;
            ImportedRows = importedRows;
        }
    }
}