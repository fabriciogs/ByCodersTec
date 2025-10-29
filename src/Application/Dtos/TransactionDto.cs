using FixedWidthParserWriter;
using System.Globalization;

namespace Application.Dtos
{
    public class TransactionDto
    {
        [FixedWidthLineField(Start = 1, Length = 1)]
        public int Type { get; set; }

        [FixedWidthLineField(Start = 2, Length = 8)]
        public string Date { get; set; }

        [FixedWidthLineField(Start = 10, Length = 10)]
        public decimal Value { get; set; }

        [FixedWidthLineField(Start = 20, Length = 11)]
        public string Cpf { get; set; }

        [FixedWidthLineField(Start = 31, Length = 12)]
        public string Card { get; set; }

        [FixedWidthLineField(Start = 43, Length = 6)]
        public string Time { get; set; }

        [FixedWidthLineField(Start = 49, Length = 14)]
        public string StoreOwner { get; set; }

        [FixedWidthLineField(Start = 63)]
        public string StoreName { get; set; }

        public DateTime OccurrenceDate => DateTime.ParseExact(Date + Time, "yyyyMMddHHmmss", new CultureInfo("pt-BR"));
    }
}