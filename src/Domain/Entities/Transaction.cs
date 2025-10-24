using System.Globalization;

namespace Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public int Type { get; private set; }
        public string Date { get; private set; }
        public decimal Value { get; private set; }
        public string Cpf { get; private set; }
        public string Card { get; private set; }
        public string Time { get; private set; }
        public string StoreOwner { get; private set; }
        public string StoreName { get; private set; }
        public DateTime DateTime => FormatDateTime(Date, Time);

        private Transaction() { }

        public Transaction(Guid id, int type, string date, decimal value, string cpf, string card, string time, string storeOwner, string storeName)
        {
            if (!IsValidType(type)) throw new ArgumentException("Invalid transaction type");
            if (value <= 0) throw new ArgumentException("Value must be positive");
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11) throw new ArgumentException("Invalid CPF");
            if (string.IsNullOrWhiteSpace(card) || card.Length != 12) throw new ArgumentException("Invalid Card");
            if (string.IsNullOrWhiteSpace(storeOwner)) throw new ArgumentException("Store owner is required");
            if (string.IsNullOrWhiteSpace(storeName)) throw new ArgumentException("Store name is required");

            Id = id;
            Type = type;
            Date = date;
            Value = value / 100;
            Cpf = cpf;
            Card = card;
            Time = time;
            StoreOwner = storeOwner.Trim();
            StoreName = storeName.Trim();
        }

        public decimal GetSignedValue() => Type switch
        {
            1 or 4 or 5 or 6 or 7 or 8 => Value, // Entrada: +
            2 or 3 or 9 => -Value, // Saída: -
            _ => 0m
        };

        private static bool IsValidType(int type) => type >= 1 && type <= 9;

        private static DateTime FormatDateTime(string dateOnly, string timeOnly) 
            => DateTime.ParseExact($"{dateOnly} {timeOnly}", "yyyyMMdd HHmmss", new CultureInfo("pt-BR"));
    }
}