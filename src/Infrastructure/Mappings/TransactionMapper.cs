using Infrastructure.DapperDataAccess.Entities;

namespace Infrastructure.Mappings
{
    internal static class TransactionMapper
    {
        public static Domain.Entities.Transaction ToEntity(this Transaction dbe)
            => new(
                  dbe.Id
                , dbe.Type
                , dbe.DateTime.ToString("yyyyMMdd")
                , dbe.Value
                , dbe.Cpf
                , dbe.Card
                , dbe.DateTime.ToString("HHmmss")
                , dbe.StoreOwner
                , dbe.StoreName
                    );

        public static Transaction ToDbEntity(this Domain.Entities.Transaction e, Guid fileImportId)
        {
            return new Transaction
            {
                Id = e.Id,
                Type = e.Type,
                DateTime = e.DateTime,
                Value = e.Value,
                Cpf = e.Cpf,
                Card = e.Card,
                StoreOwner = e.StoreOwner,
                StoreName = e.StoreName,
                ImportDate = DateTime.Now,
                FileImportId = fileImportId,
            };
        }
    }
}