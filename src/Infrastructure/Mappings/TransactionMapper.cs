using Infrastructure.DapperDataAccess.Entities;

namespace Infrastructure.Mappings
{
    internal static class TransactionMapper
    {
        public static Domain.Entities.Transaction ToEntity(this Transaction dbe)
            => new(
                dbe.ImportFileId
                , dbe.Id
                , dbe.TransactionTypeId
                , dbe.OccurrenceDate
                , dbe.Value
                , dbe.Cpf
                , dbe.Card
                , dbe.StoreOwner
                , dbe.StoreName
                    );

        public static Transaction ToDbEntity(this Domain.Entities.Transaction e)
        {
            return new Transaction
            {
                Id = e.Id,
                ImportFileId = e.ImportFileId,
                TransactionTypeId = e.Type,
                OccurrenceDate = e.OccurrenceDate,
                Value = e.Value,
                Cpf = e.Cpf,
                Card = e.Card,
                StoreOwner = e.StoreOwner,
                StoreName = e.StoreName,
            };
        }
    }
}