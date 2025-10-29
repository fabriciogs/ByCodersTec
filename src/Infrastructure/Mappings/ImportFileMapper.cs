using Infrastructure.DapperDataAccess.Entities;

namespace Infrastructure.Mappings
{
    internal static class ImportFileMapper
    {
        public static Domain.Entities.ImportFile ToEntity(this ImportFile dbe)
            => new(dbe.Id, dbe.FileName, dbe.UserId, dbe.TotalRows, dbe.ImportedRows);

        public static ImportFile ToDbEntity(this Domain.Entities.ImportFile e)
        {
            return new ImportFile
            {
                Id = e.Id,
                ImportDate = e.ImportDate,
                FileName = e.FileName,
                UserId = e.UserId,
                TotalRows = e.TotalRows,
                ImportedRows = e.ImportedRows
            };
        }
    }
}